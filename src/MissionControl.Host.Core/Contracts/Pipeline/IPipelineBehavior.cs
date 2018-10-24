using System.Threading.Tasks;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core.Contracts.Pipeline
{
    public interface IPipelineBehavior<in T> where T : CliCommand
    {
        Task<CliResponse> Process(T command, CliHandlerDelegate next);
    }
}