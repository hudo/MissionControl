using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core
{
    public class ConHost : IConHost
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
                // find handler and execute
                
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

    public interface IConHost
    {
        string ClientId { get; }
        Task<CliResponse> Execute(CliCommand command);
    }

    internal interface IConHostFactory
    {
        IConHost Create(string clientId);
    }

    internal class ConHostFactory : IConHostFactory
    {
        public IConHost Create(string clientId)
        {
            return new ConHost(clientId);
        }
    }


}