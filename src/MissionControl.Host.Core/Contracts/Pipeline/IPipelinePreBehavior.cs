using System.Threading.Tasks;

namespace MissionControl.Host.Core.Contracts.Pipeline
{
    public interface IPipelinePreBehavior<in T> where T : CliCommand
    {
        Task Process(T command);
    }
}