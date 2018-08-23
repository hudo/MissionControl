using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MissionControl.Host.Core
{
    public class ConHost
    {
        private readonly ConcurrentQueue<CliCommand> _inbox  = new ConcurrentQueue<CliCommand>();

        public ConHost(string clientId)
        {
            ClientId = clientId;
        }
        
        
        public string ClientId { get;  }

        public Task<CliResponse> Execute(CliCommand command)
        {
            return Task.FromResult(new TextResponse("ok") as CliResponse);
        }
    }


}