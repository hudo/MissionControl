using System.Threading.Tasks;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core.Contracts
{
    internal class PingHandler : ICliCommandHandler<PingCommand>
    {
        public Task<CliResponse> Handle(PingCommand command)
        {
            return Task.FromResult(new TextResponse($"Pong to {command.Name}") as CliResponse);
        }
    }
}