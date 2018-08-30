using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
                // find handler and execute

                try
                {
                    var stopwatch = Stopwatch.StartNew();
                    
                    var handlertype = typeof(ICliCommandHandler<>).MakeGenericType(command.command.GetType());

                    var service = _serviceProvider.GetService(handlertype);

                    Task<CliResponse> task = service.GetType().GetMethod("Handle").Invoke(service, new[] {command.command}) as Task<CliResponse>;

                    var response = await task;
                    
                    stopwatch.Stop();
                    _logger.LogInformation($"Command [{command.command.GetType().Name}] executed in {stopwatch.ElapsedMilliseconds}ms");

                    command.completionSource.SetResult(response);                    
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error executing command [{command.command.GetType().Name}]: {e.Unwrap().Message}");
                    
                    command.completionSource.SetResult(new ErrorResponse(e.Unwrap().Message));
                }
            }
        }

        public Task<CliResponse> Execute(CliCommand command)
        { 
            if (_inbox.IsAddingCompleted)
                throw new ApplicationException("ConHost stopped"); 

            _logger.LogDebug($"Command [{command.GetType().Name}] scheduled for processing: {JsonConvert.SerializeObject(command)}");

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
        private readonly IServiceProvider _serviceProvider;

        public ConHostFactory(ILogger<ConHost> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public IConHost Create(string clientId)
        {
            return new ConHost(clientId, _serviceProvider, _logger);
        }
    }

}