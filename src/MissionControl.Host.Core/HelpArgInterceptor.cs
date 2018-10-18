using System.Threading.Tasks;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core
{
    public interface IInterceptor<T> where T:CliCommand
    {
        Task<CliResponse> Intercept(ICliCommandHandler<T> handler, T command);
    }
    
    internal class HelpArgInterceptor<T> : IInterceptor<T> where T : CliCommand
    {
        public Task<CliResponse> Intercept(ICliCommandHandler<T> handler, T command)
        {
            return handler.Handle(command);
        }
    }
}