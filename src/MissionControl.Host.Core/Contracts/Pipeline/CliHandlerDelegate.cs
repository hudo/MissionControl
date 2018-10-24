using System.Threading.Tasks;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core.Contracts.Pipeline
{
    public delegate Task<CliResponse> CliHandlerDelegate();
}