using System.Threading.Tasks;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core
{
    public interface IConHost
    {
        string ClientId { get; }
        Task<CliResponse> Execute(CliCommand command);
    }
}