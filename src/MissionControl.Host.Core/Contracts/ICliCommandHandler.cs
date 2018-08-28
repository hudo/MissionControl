using System.Threading.Tasks;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core.Contracts
{
    public interface ICliCommandHandler<in T> where T : CliCommand
    {
        Task<CliResponse> Handle(T command);
    }
}