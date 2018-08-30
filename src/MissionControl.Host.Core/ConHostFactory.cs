using System;
using Microsoft.Extensions.Logging;

namespace MissionControl.Host.Core
{
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