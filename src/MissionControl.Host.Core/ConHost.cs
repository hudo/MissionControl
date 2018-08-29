using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core
{
    public class ConHost : IConHost
    {
        private readonly ILogger<ConHost> _logger;

        private readonly BlockingCollection<(CliCommand command, TaskCompletionSource<CliResponse> completionSource)> _inbox  
            = new BlockingCollection<(CliCommand, TaskCompletionSource<CliResponse>)>();

        public ConHost(string clientId, ILogger<ConHost> logger)
        {
            _logger = logger;
            ClientId = clientId;
            
            Task.Run(ProcessInbox);
        }

        public string ClientId { get;  }

        private async Task ProcessInbox()
        {
            _logger.LogDebug("Started processing ConHost commands");

            foreach (var command in _inbox.GetConsumingEnumerable())
            {
                // find handler and execute
                
                command.completionSource.SetResult(new TextResponse("exec: " + command.GetType()));

                await Task.Delay(100);
            }
        }

        public Task<CliResponse> Execute(CliCommand command)
        { 
            if (_inbox.IsAddingCompleted)
                throw new ApplicationException("ConHost stopped"); 

            _logger.LogDebug("Command scheduled for processing");

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
        private readonly ILogger<ConHost> _logger;

        public ConHostFactory(ILogger<ConHost> logger)
        {
            _logger = logger;
        }

        public IConHost Create(string clientId)
        {
            return new ConHost(clientId, _logger);
        }
    }


}