using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Utilities;

namespace MissionControl.Host.Core
{
    internal class RequestParser : IRequestParser
    {
        private readonly ILogger<RequestParser> _logger;
        private readonly ICommandTypesCatalog _catalog;

        public RequestParser(ICommandTypesCatalog catalog, ILogger<RequestParser> logger)
        {
            _logger = Guard.NotNull(logger, nameof(logger));
            _catalog = Guard.NotNull(catalog, nameof(catalog));
        }

        public Maybe<CliCommand> Parse(Request request)
        {
            var (type, attribs) = _catalog.GetTypeByCommandName(request.Command);

            if (type == null)
                throw new ArgumentException($"Command '{request.Command}' not found.");

            var command = Activator.CreateInstance(type) as CliCommand;

            if (command == null)
                throw new Exception("Command needs to inherit CliCommand");

            command.CorrelationId = request.CorrelationId;

            foreach (var arg in request.Args)
            {
                if (arg.Contains("="))
                {
                    var parts = arg.Split('=');
                    var key = parts[0].TrimStart('-');
                    var propertyInfo = type.GetProperty(key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);

                    try
                    {
                        if (propertyInfo != null)
                            propertyInfo.SetValue(command, Convert.ChangeType(parts[1], propertyInfo.PropertyType), null);
                        else
                            _logger.LogTrace($"Field '{key}' not found on type '{type.Name}'");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Error mapping value {key} to {type.Name}");
                    }
                }
                else
                {
                    // design how to handle args without name, like:
                    // > copy arg1 arg2
                }
            }

            return command;
        }
    }
    
    // deserialize request to DTO props
    internal interface IRequestParser
    {
        Maybe<CliCommand> Parse(Request request);
    }
}