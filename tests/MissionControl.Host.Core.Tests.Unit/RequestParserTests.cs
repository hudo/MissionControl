using System;
using Microsoft.Extensions.Logging;
using MissionControl.Host.Core.Contracts;
using Moq;
using Shouldly;
using Xunit;

namespace MissionControl.Host.Core.Tests.Unit
{
    public class RequestParserTests
    {
        private readonly Mock<ICommandTypesCatalog> _typeCatalogMock = new Mock<ICommandTypesCatalog>();
        private readonly RequestParser _requestParser;

        public RequestParserTests()
        {
            _typeCatalogMock
                .Setup(x => x.RegisteredCommands)
                .Returns(new[]
                {
                    new CommandRegistration {Name = "test", Type = typeof(TestCommand), Attribute = new CliCommandAttribute("test")},
                    new CommandRegistration {Name = "test2", Type = typeof(Test2Command), Attribute = new CliCommandAttribute("test2")}, 
                });
                
            _requestParser = new RequestParser(_typeCatalogMock.Object, Mock.Of<ILogger<RequestParser>>());
        }

        [Fact]
        public void Parse_request_with_no_args()
        {
            var command = _requestParser.Parse(new Request { Command = "test" });
            
            command.IsSome.ShouldBeTrue();
            command.Value.ShouldBeOfType<TestCommand>();
        }

        [Fact]
        public void Type_not_found_throws_exception()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var command = _requestParser.Parse(new Request {Command = "foo"});
            });
        }

        [Fact]
        public void Parse_request_with_arguments()
        {
            var command = _requestParser.Parse(new Request
            {
                Command = "test",
                Args = new []{ "prop1=foo", "Prop2=5", "-prop3=4.2", "foo=bar"}
            });
            
            command.IsSome.ShouldBeTrue();
            command.Value.ShouldBeOfType<TestCommand>();

            var testCommand = command.Value as TestCommand;
            testCommand.Prop1.ShouldBe("foo");
            testCommand.Prop2.ShouldBe(5);
            testCommand.Prop3.ShouldBe(4.2m);
        }

        [Fact]
        public void Ignore_invalid_arg_values()
        {
            var command = _requestParser.Parse(new Request
            {
                Command = "test",
                Args = new []{ "prop1=1", "Prop2=foo", "-prop3=bar"}
            });
            
            command.IsSome.ShouldBeTrue();
            command.Value.ShouldBeOfType<TestCommand>();

            var testCommand = command.Value as TestCommand;
            testCommand.Prop1.ShouldBe("1");
            testCommand.Prop2.ShouldBe(0);
            testCommand.Prop3.ShouldBe(0);
        }

        [Fact]
        public void Map_empty_value_to_boolean()
        {
            var command = _requestParser.Parse(new Request
            {
                Command = "test",
                Args = new []{ "-prop4"}
            });
            
            command.IsSome.ShouldBeTrue();
            command.Value.ShouldBeOfType<TestCommand>();

            var testCommand = command.Value as TestCommand;
            testCommand.Prop4.ShouldBeTrue();
        }

        [Fact]
        public void Skip_CorrelationId_args()
        {
            var command = _requestParser.Parse(new Request
            {
                Command = "test",
                Args = new[] { "-CorrelationId=123" }
            });

            command.Value.ClientId.ShouldBeNullOrEmpty();
        }

        [Fact]
        public void Exception_on_missing_required_arg()
        {
            var cmd = _requestParser.Parse(new Request
            {
                Command = "test2",
                Args = new[] {"-required=123;-notRequired=123"}
            });
            
            cmd.IsSome.ShouldBeTrue();

            Assert.Throws<Exception>(() => { 
                _requestParser.Parse(new Request
                {
                    Command = "test2",
                    Args = new[] {"-notRequired=foo"}
                });
            });
        }

        private class TestCommand : CliCommand
        {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
            public decimal Prop3 { get; set; }
            public bool Prop4 { get; set; }
        }

        private class Test2Command : CliCommand
        {
            [CliArg(required: true)] public string Required { get; set; }
            [CliArg(required: false)] public string NotRequired { get; set; }
        }
    }
}