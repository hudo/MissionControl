using System.Threading.Tasks;
using MissionControl.Host.Core.Contracts;

namespace MissionControl.Host.Core.Pipeline
{
    public interface IPipelinePreBehavior<in T> where T : CliCommand
    {
        Task Process(T command);
    }
}