using System;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace MissionControl.Host.Core.Tests.Unit
{
    public class RequestParserTests
    {
        private Mock<ICommandTypesCatalog> _typeCatalogMock = new Mock<ICommandTypesCatalog>();
        private RequestParser _requestParser;

        public RequestParserTests()
        {
            _typeCatalogMock
                .Setup(x => x.GetTypeByCommandName(It.Is<string>(name => name == "test")))
                .Returns((typeof(TestCommand), new CommandTextAttribute("test")));
            
            _requestParser = new RequestParser(_typeCatalogMock.Object, Mock.Of<ILogger<RequestParser>>());
        }

        [Fact]
        public void Parse_request_with_no_args()
        {
            var command = _requestParser.Parse(new Request { Command = "test" });
            
            command.ShouldNotBeNull();
            command.ShouldBeOfType<TestCommand>();
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
                Args = new []{ "prop1=foo", "Prop2=5", "-prop3=4.2"}
            });
            
            command.ShouldNotBeNull();
            command.ShouldBeOfType<TestCommand>();

            var testCommand = command as TestCommand;
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
            
            command.ShouldNotBeNull();
            command.ShouldBeOfType<TestCommand>();

            var testCommand = command as TestCommand;
            testCommand.Prop1.ShouldBe("1");
            testCommand.Prop2.ShouldBe(0);
            testCommand.Prop3.ShouldBe(0);
        }

        private class TestCommand : CliCommand
        {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
            public decimal Prop3 { get; set; }
        }
    }
}