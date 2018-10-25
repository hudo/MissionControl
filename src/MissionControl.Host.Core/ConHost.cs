using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Contracts.Pipeline;
using MissionControl.Host.Core.Responses;
using MissionControl.Host.Core.Utilities;
using Newtonsoft.Json;

namespace MissionControl.Host.Core
{
    /// <summary>
    /// Host for one terminal window. Has list of incoming commands and executes them in FIFO order
    /// </summary>
    public class ConHost : IConHost, IDisposable
    {
        private readonly ServiceFactory _serviceFactory;
        private readonly ILogger<ConHost> _logger;

        private readonly BlockingCollection<(CliCommand command, TaskCompletionSource<CliResponse> completionSource)> _inbox  
            = new BlockingCollection<(CliCommand, TaskCompletionSource<CliResponse>)>();

        // todo: how to handle history in clusters?
        private readonly List<CliCommand> _history = new List<CliCommand>();

        public ConHost(string clientId, ServiceFactory serviceFactory, ILogger<ConHost> logger)
        {
            _serviceFactory = Guard.NotNull(serviceFactory, nameof(serviceFactory));
            _logger = Guard.NotNull(logger, nameof(logger));
            ClientId = clientId;
            
            Task.Run(ProcessInbox);
        }

        public string ClientId { get;  }

        private async Task ProcessInbox()
        {
            _logger.LogDebug("Started processing ConHost commands");
            foreach (var item in _inbox.GetConsumingEnumerable())
            {
                var cmdName = item.command.GetType().Name;
                try
                {
                    var stopwatch = Stopwatch.StartNew();

                    var handleTask = ResolveHandler(item.command, item.completionSource, cmdName);
                    
                    if (handleTask == null) continue;
                    
                    var response = await handleTask;
                    
                    stopwatch.Stop();
                    _logger.LogInformation($"Command [{cmdName}] executed in {stopwatch.ElapsedMilliseconds}ms");
                    
                    item.completionSource.SetResult(response);
                    
                    Record(item.command);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error executing command [{cmdName}]: {e.Unwrap().Message}");
                    
                    item.completionSource.SetResult(new ErrorResponse(e.Unwrap().Message));
                }
            }
        }

        private void Record(CliCommand command)
        {
            _history.Add(command);

            if (_history.Count > 100)
            {
                _history.RemoveAt(0);
            }
        }

        private Task<CliResponse> ResolveHandler(CliCommand command, TaskCompletionSource<CliResponse> completionSource, string cmdName)
        {
            var handler = _serviceFactory(typeof(ICliCommandHandler<>).MakeGenericType(command.GetType()));

            if (handler == null)
            {
                _logger.LogWarning($"Handler for command [{cmdName}] not found.");
                completionSource.SetResult(new ErrorResponse($"Handler not found for command [{cmdName}]"));
                return null;
            }

            try
            {
                var pipelineTask = (Task<CliResponse>)this.GetType()
                    .GetMethod(nameof(ComposePipeline), BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(command.GetType())
                    .Invoke(this, new[] { command });

                return pipelineTask;
            }
            catch (Exception e)
            {
                var message = e.Unwrap().Message;
                _logger.LogError($"Error composing pipeline: {message}");
                completionSource.SetResult(new ErrorResponse($"Internal error: {message}"));
                return null;
            }
        }

        private Task<CliResponse> ComposePipeline<T>(T command) where  T : CliCommand
        {
            CliHandlerDelegate handlerDelegate = () => _serviceFactory.GetInstance<ICliCommandHandler<T>>().Handle(command);

            return _serviceFactory.GetInstances<IPipelineBehavior<T>>()
                .Reverse()
                .Aggregate(handlerDelegate, (next, pipeline) => () => pipeline.Process(command, next))();
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

        public void Dispose()
        {
            _history.Clear();
            _inbox.CompleteAdding();
            _inbox.Dispose();
        }
    }
}