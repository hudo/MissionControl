using System.Threading.Tasks;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.AspnetCore.Tests.Integration
{
    [CliCommand("ping", help:"Ping Pong")]
    internal class PingCommand : CliCommand
    {
        [CliArg(required: true, help: "Say your name")]
        public string Name { get; set; }
    }

    internal class PingHandler : ICliCommandHandler<PingCommand>
    {
        public Task<CliResponse> Handle(PingCommand command)
        {
            return Task.FromResult(new TextResponse($"Pong to {command.Name}") as CliResponse);
        }
    }
}