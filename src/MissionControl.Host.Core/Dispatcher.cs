using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MissionControl.Host.Core.Responses;
using MissionControl.Host.Core.Utilities;

namespace MissionControl.Host.Core
{
    internal class Dispatcher : IDispatcher, IDisposable
    {
        private readonly IConHostFactory _conHostFactory;
        private readonly IRequestParser _parser;
        private readonly ILogger<Dispatcher> _logger;

        private static readonly object _lock = new object();
        private readonly Timer _timer;
        
        private readonly List<ConHostRegistration> _hostsRegistrations = new List<ConHostRegistration>();

        public Dispatcher(IRequestParser parser, IConHostFactory conHostFactory,  ILogger<Dispatcher> logger)
        {
            _conHostFactory = Guard.NotNull(conHostFactory, nameof(conHostFactory));
            _parser = Guard.NotNull(parser, nameof(parser));
            _logger = Guard.NotNull(logger, nameof(logger));

            _timer = new Timer(OnTimerTick, null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
        }

        private void OnTimerTick(object state)
        {
            lock (_lock) _hostsRegistrations.RemoveAll(r => DateTime.UtcNow.Subtract(r.LastUpdated) > TimeSpan.FromMinutes(20));
        }

        public async Task<CliResponse> Invoke(Request request)
        {
            var registration = _hostsRegistrations.FirstOrDefault(x => x.ConHost.ClientId == request.CorrelationId);

            if (registration == null)
            {
                registration = new ConHostRegistration
                {
                    ConHost = _conHostFactory.Create(request.CorrelationId), 
                };
                
                lock(_lock) _hostsRegistrations.Add(registration);
                _logger.LogInformation($"New ConHost created for client {request.CorrelationId}");
            }

            try
            {
                lock (_lock) registration.LastUpdated = DateTime.UtcNow;
                
                var command = _parser.Parse(request);
                
                if (command.IsNull)
                {
                    // send syntax error back?
                    return new ErrorResponse("Command not found");
                }
                
                return await registration.ConHost.Execute(command.Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error executing request '{request.Command}': {e.Message}");
                return new ErrorResponse($"Error: {e.Unwrap().Message}");
            }
        }

        private class ConHostRegistration
        {
            public IConHost ConHost;
            public DateTime LastUpdated;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
    
    
    public interface IDispatcher
    {
        Task<CliResponse> Invoke(Request request);
    }
}