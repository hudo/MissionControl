using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core
{
    public class OutputBuffer
    {
        private static readonly object Lock = new object();
        
        private readonly ConcurrentDictionary<string, BlockingCollection<CliResponse>> _outbox = new ConcurrentDictionary<string, BlockingCollection<CliResponse>>();
        
        
        public void Send(CliResponse response) => GetOutboxForClient(response.ClientId).Add(response);

        public async Task<CliResponse> GetNextResponse(string clientId)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var outbox = GetOutboxForClient(clientId);

            
        }

        private BlockingCollection<CliResponse> GetOutboxForClient(string clientId)
        {
            if (!_outbox.TryGetValue(clientId, out var outbox))
            {
                outbox = new BlockingCollection<CliResponse>();
                _outbox.TryAdd(clientId, outbox);
            }

            return outbox;
        }
                
    }
}