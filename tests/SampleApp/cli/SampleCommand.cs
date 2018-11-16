using System.Threading.Tasks;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;

namespace SampleApp.cli
{
    [CliCommand("sample", "Sample command with Foo argument")]
    public class SampleCommand : CliCommand
    {
        [CliArg(true, "Foo prop that's required", skip: false)]
        public string Foo { get; set; }
    }

    [CliCommand("sample2", "Sample 2 command without arguments")]
    public class Sample2Command : CliCommand { }

    public class SampleHandler: ICliCommandHandler<SampleCommand>
    {
        public Task<CliResponse> Handle(SampleCommand command)
        {
            return Task.FromResult(new TextResponse($"sample handler response: {command.Foo}") as CliResponse);
        }
    }
}