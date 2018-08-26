using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MissionControl.Host.Core.Utilities;

namespace MissionControl.Host.Core
{
    internal class Dispatcher : IDispatcher
    {
        private readonly IRequestParser _parser;
        private readonly ILogger<Dispatcher> _logger;
        
        private readonly List<ConHost> _hosts = new List<ConHost>();

        public Dispatcher(IRequestParser parser, ILogger<Dispatcher> logger)
        {
            _parser = Guard.NotNull(parser, nameof(parser));
            _logger = Guard.NotNull(logger, nameof(logger));
        }

        public async Task<CliResponse> Invoke(Request request)
        {
            var conHost = _hosts.FirstOrDefault(x => x.ClientId == request.ClientId);

            if (conHost == null)
            {
                // lock?

                conHost = new ConHost(request.ClientId);
                _hosts.Add(conHost);
            }

            try
            {
                var command = _parser.Parse(request);

                return await conHost.Execute(command);
            }
            catch (Exception e)
            {

            }
        }
    }
    
    
    public interface IDispatcher
    {
        Task<CliResponse> Invoke(Request request);
    }
}