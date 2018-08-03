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
            var response = await handler.Handle(new CliCommand());

            var disposable = response.Observable.Subscribe(
                next => { },
                err => { });
            
            disposable.Dispose();
        }
    }
    
    public class TestCommandHandler : ICliCommandHandler<CliCommand, SagaResponse>
    {
        public async Task<SagaResponse> Handle(CliCommand command)
        {
            await Task.Delay(0);
            
            return new SagaResponse(Observable.Create<CliResponse>(observer =>
            {
                observer.OnNext(new TextResponse());
                observer.OnCompleted();
                return Task.CompletedTask;
            }));
        }
    }
}
