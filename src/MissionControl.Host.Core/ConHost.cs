using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;
using MissionControl.Host.Core.Utilities;
using Newtonsoft.Json;

namespace MissionControl.Host.Core
{
    public class ConHost : IConHost
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ConHost> _logger;

        private readonly BlockingCollection<(CliCommand command, TaskCompletionSource<CliResponse> completionSource)> _inbox  
            = new BlockingCollection<(CliCommand, TaskCompletionSource<CliResponse>)>();

        public ConHost(string clientId, IServiceProvider serviceProvider, ILogger<ConHost> logger)
        {
            _serviceProvider = serviceProvider;
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
                var cmdName = command.command.GetType().Name;
                try
                {
                    var stopwatch = Stopwatch.StartNew();
                    var handlerType = typeof(ICliCommandHandler<>).MakeGenericType(command.command.GetType());
                    var handler = _serviceProvider.GetService(handlerType);

                    if (handler == null)
                    {
                        _logger.LogWarning($"Handler for command [{cmdName}] not found.");
                        command.completionSource.SetResult(new ErrorResponse($"Handler not found for command [{cmdName}]"));
                        continue;
                    }

                    var handleTask = handler.GetType().GetMethod("Handle").Invoke(handler, new object[] {command.command}) as Task<CliResponse>;
                    var response = await handleTask;

                    stopwatch.Stop();
                    _logger.LogInformation($"Command [{cmdName}] executed in {stopwatch.ElapsedMilliseconds}ms");
                    
                    command.completionSource.SetResult(response);                    
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error executing command [{cmdName}]: {e.Unwrap().Message}");
                    
                    command.completionSource.SetResult(new ErrorResponse(e.Unwrap().Message));
                }
            }
        }

        public Task<CliResponse> Execute(CliCommand command)
        { 
            if (_inbox.IsAddingCompleted)
                throw new ApplicationException("ConHost stopped"); 
            
            var completionSource = new TaskCompletionSource<CliResponse>();
            
            _inbox.Add((command, completionSource));
            
            _logger.LogDebug($"Command [{command.GetType().Name}] scheduled for processing: {JsonConvert.SerializeObject(command)}");
            
            return completionSource.Task;
        }
    }

    public interface IConHost
    {
        string ClientId { get; }
        Task<CliResponse> Execute(CliCommand command);
    }

    
}