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
        private readonly ServiceFactory _serviceFactory;

        public ConHostFactory(ILogger<ConHost> logger, ServiceFactory serviceFactory)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        public IConHost Create(string clientId)
        {
            return new ConHost(clientId, _serviceFactory, _logger);
        }
    }
}