﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;
using MissionControl.Host.Core.Utilities;

namespace MissionControl.Host.Core
{
    /// <summary>
    /// Dispatches request to console host. Keeps one console host per terminal window
    /// </summary>
    internal class Dispatcher : IDispatcher, IDisposable
    {
        private readonly IConHostFactory _conHostFactory;
        private readonly IRequestParser _parser;
        private readonly ILogger<Dispatcher> _logger;

        private static readonly object Lock = new object();
        private readonly Timer _timer;
        
        private readonly List<ConHostRegistration> _hostsRegistrations = new List<ConHostRegistration>();

        public Dispatcher(IRequestParser parser, IConHostFactory conHostFactory,  ILogger<Dispatcher> logger)
        {
            _conHostFactory = Guard.NotNull(conHostFactory, nameof(conHostFactory));
            _parser = Guard.NotNull(parser, nameof(parser));
            _logger = Guard.NotNull(logger, nameof(logger));

            _timer = new Timer(OnTimerTick, null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// Cleanup disconnected consoles (no received commands more than 20mins) 
        /// </summary>
        private void OnTimerTick(object state)
        {
            lock (Lock) _hostsRegistrations.RemoveAll(r => DateTime.UtcNow.Subtract(r.LastUpdated) > TimeSpan.FromMinutes(20));
        }

        public async Task<CliResponse> Invoke(Request request)
        {
            var registration = _hostsRegistrations.FirstOrDefault(x => x.ConHost.ClientId == request.ClientId);

            if (registration == null)
            {
                registration = new ConHostRegistration
                {
                    ConHost = _conHostFactory.Create(request.ClientId), 
                };
                
                lock(Lock) _hostsRegistrations.Add(registration);
                _logger.LogInformation($"New ConHost created for client {request.ClientId}");
            }

            try
            {
                lock (Lock) registration.LastUpdated = DateTime.UtcNow;
                
                var command = _parser.Parse(request);
                
                if (command.IsNull)
                {
                    // send syntax error back?
                    return new ErrorResponse("Command not found");
                }
                
                var response = await registration.ConHost.Execute(command.Value);
                return response;
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
            _hostsRegistrations.Clear();
        }
    }
    
    
    public interface IDispatcher
    {
        Task<CliResponse> Invoke(Request request);
    }
}