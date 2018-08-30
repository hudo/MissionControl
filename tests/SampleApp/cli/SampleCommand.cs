using System.Threading.Tasks;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;

namespace SampleApp.cli
{
    [CliCommand("sample")]
    public class SampleCommand : CliCommand
    {
        public string Foo { get; set; }
    }

    [CliCommand("sample2")]
    public class Sample2Command : CliCommand { }

    public class SampleHandler: ICliCommandHandler<SampleCommand>
    {
        public Task<CliResponse> Handle(SampleCommand command)
        {
            return Task.FromResult(new TextResponse("sample handler response") as CliResponse);
        }
    }
}