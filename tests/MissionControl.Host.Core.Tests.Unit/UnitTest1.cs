using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MissionControl.Host.Core.Tests.Unit
{
    public class Playground
    {
        [Fact]
        public async Task Test1()
        {
            var handler = new TestCommandHandler();
            var response = await handler.Handle(new HelloWorldCommand());

            /*var disposable = response.Observable.Subscribe(
                next => { },
                err => { });*/
        }
    }
    
    public class TestCommandHandler : ICliCommandHandler<HelloWorldCommand, TextResponse>
    {
        public Task<TextResponse> Handle(HelloWorldCommand command)
        {
            return Task.FromResult(new TextResponse($"hi {command.Name}"));
        }
    }
}
