using System.Threading.Tasks;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;

namespace SampleApp.cli
{
    
    [CliCommand("multi-resp", "Multiple responses example")]
    public class MultipleResponsesCommand : CliCommand {  }
    
    public class MultipleResponsesHandler : ICliCommandHandler<MultipleResponsesCommand>
    {
        public async Task<CliResponse> Handle(MultipleResponsesCommand command)
        {
            await Task.Delay(1000);
            
            return new MultipleResponses(async yield =>
            {
                await yield.ReturnAsync("Response 1");
                await Task.Delay(2000);
                await yield.ReturnAsync("Response 2");
                await Task.Delay(2000);
                await yield.ReturnAsync("Response 3 - done.");
            });
        }
    }
}