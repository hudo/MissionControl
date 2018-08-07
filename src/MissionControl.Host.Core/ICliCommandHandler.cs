using System.Threading.Tasks;

namespace MissionControl.Host.Core
{
    public interface ICliCommandHandler<in T,R> 
        where T : CliCommand
        where R : CliResponse
    {
        Task<R> Handle(T command);
    }
}