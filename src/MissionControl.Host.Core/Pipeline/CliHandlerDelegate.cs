using System.Threading.Tasks;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core.Pipeline
{
    internal delegate Task<CliResponse> CliHandlerDelegate();
}