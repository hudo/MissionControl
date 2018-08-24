using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MissionControl.Host.Core
{
    public class ConHost
    {
        private readonly BlockingCollection<(CliCommand command, TaskCompletionSource<CliResponse> completionSource)> _inbox  
            = new BlockingCollection<(CliCommand, TaskCompletionSource<CliResponse>)>();

        public ConHost(string clientId)
        {
            ClientId = clientId;
            
            ProcessInbox();
        }

        public string ClientId { get;  }

        private async Task ProcessInbox()
        {
            foreach (var command in _inbox.GetConsumingEnumerable())
            {
                command.completionSource.SetResult(new TextResponse("hi"));

                await Task.Delay(100);
            }
        }

        public Task<CliResponse> Execute(CliCommand command)
        {
            if (_inbox.IsAddingCompleted)
                throw new ApplicationException("ConHost stopped"); 

            var completionSource = new TaskCompletionSource<CliResponse>();

            _inbox.Add((command, completionSource));

            return completionSource.Task;
        }
    }


}