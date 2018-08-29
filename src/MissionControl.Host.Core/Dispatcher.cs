using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MissionControl.Host.Core.Responses;
using MissionControl.Host.Core.Utilities;

namespace MissionControl.Host.Core
{
    internal class Dispatcher : IDispatcher
    {
        private readonly IConHostFactory _conHostFactory;
        private readonly IRequestParser _parser;
        private readonly ILogger<Dispatcher> _logger;
        
        private readonly List<IConHost> _hosts = new List<IConHost>();

        public Dispatcher(IRequestParser parser, IConHostFactory conHostFactory,  ILogger<Dispatcher> logger)
        {
            _conHostFactory = Guard.NotNull(conHostFactory, nameof(conHostFactory));
            _parser = Guard.NotNull(parser, nameof(parser));
            _logger = Guard.NotNull(logger, nameof(logger));
        }

        public async Task<CliResponse> Invoke(Request request)
        {
            var conHost = _hosts.FirstOrDefault(x => x.ClientId == request.ClientId);

            if (conHost == null)
            {
                // lock?
                conHost = _conHostFactory.Create(request.ClientId);
                _hosts.Add(conHost);

                _logger.LogInformation($"New ConHost created for client {request.ClientId}");
            }

            try
            {
                var command = _parser.Parse(request);

                if (command.IsNull)
                {
                    // send syntax error back?
                    return new ErrorResponse("Command not found");
                }
                
                return await conHost.Execute(command.Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error executing request '{request.Command}': {e.Message}");
                
                return new ErrorResponse($"Error: {e.Unwrap().Message}");
            }
        }
    }
    
    
    public interface IDispatcher
    {
        Task<CliResponse> Invoke(Request request);
    }
}