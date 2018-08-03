using System.Threading.Tasks;

namespace MissionControl.Host.Core
{
    public interface ICliCommandHandler<in T,R> 
        where T : CliCommand
        where R : CliResponse
    {
        // https://www.interact-sw.co.uk/iangblog/2013/11/29/async-yield-return
        
        Task<R> Handle(T command);
    }
}