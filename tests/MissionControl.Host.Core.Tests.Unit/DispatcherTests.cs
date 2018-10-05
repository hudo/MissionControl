using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;
using Moq;
using Shouldly;
using Xunit;

namespace MissionControl.Host.Core.Tests.Unit
{
    public class DispatcherTests
    {
        private readonly Mock<IRequestParser> _parserMock = new Mock<IRequestParser>();
        private readonly Mock<IConHostFactory> _hostFactoryMock = new Mock<IConHostFactory>();

        private readonly Dispatcher _dispatcher;

        public DispatcherTests()
        {
            _hostFactoryMock
                .Setup(x => x.Create(It.IsAny<string>()))
                .Returns((string id) => new ConHostMock(id))
                .Verifiable();
            
            _dispatcher = new Dispatcher(_parserMock.Object, _hostFactoryMock.Object, Mock.Of<ILogger<Dispatcher>>());
        }

        [Fact]
        public async Task Reuse_ConHost_for_same_clientId()
        {
            var resp1 = await _dispatcher.Invoke(new Request {ClientId = "1"});
            var resp2 = await _dispatcher.Invoke(new Request {ClientId = "1"});

            _hostFactoryMock
                .Verify(x => x.Create(It.IsAny<string>()), Times.Once);

            resp1.ShouldNotBeNull();
            resp2.ShouldNotBeNull();
            resp1.ShouldBeOfType<ErrorResponse>();
        }

        [Fact]
        public async Task Create_new_ConHost_for_different_ClientId()
        {
            await _dispatcher.Invoke(new Request {ClientId = "1"});
            await _dispatcher.Invoke(new Request {ClientId = "2"});
            
            _hostFactoryMock
                .Verify(x => x.Create(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Execute_ConHost_request()
        {
            _parserMock
                .Setup(x => x.Parse(It.IsAny<Request>()))
                .Returns(new TestCommand());

            var response = await _dispatcher.Invoke(new Request());

            response.ShouldBeOfType<TestResponse>();
        }
        
        private class ConHostMock : IConHost
        {
            public ConHostMock(string clientId)
            {
                ClientId = clientId;
            }
            
            public string ClientId { get; }
            
            public Task<CliResponse> Execute(CliCommand command)
            {
                return Task.FromResult(new TestResponse() as CliResponse);
            }
        }
        
        private class TestResponse : CliResponse { }
        private class TestCommand : CliCommand { }
    }
}