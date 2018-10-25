using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Utilities;

namespace MissionControl.Host.Core
{
    /// <summary>
    /// Takes Request with raw data and creates specific command dto
    /// </summary>
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
            var registration = _catalog.FindCommandByName(request.Command);

            if (registration.IsNull)
                throw new ArgumentException($"Command '{request.Command}' not found.");

            var command = Activator.CreateInstance(registration.Value.Type) as CliCommand;

            if (command == null)
                throw new Exception("Command needs to inherit CliCommand");

            command.CorrelationId = request.ClientId;

            ParseArguments(request, registration.Value.Type, command);

            return command;
        }

        private void ParseArguments(Request request, Type type, CliCommand command)
        {
            foreach (var arg in request.Args)
            {
                var key = "";
                try
                {
                    // todo: ugly, refactor!
                    // implement simple tokenizer and parser
                    
                    var parts = arg.Split('=');
                    key = parts[0].TrimStart('-');
                    var propertyInfo = type.GetProperty(key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);

                    if (propertyInfo != null)
                    {
                        var attribute = propertyInfo.GetCustomAttribute<CliArgAttribute>();

                        if (attribute?.Skip == true)
                            continue;

                        var value = ExtractValue(parts, propertyInfo);

                        propertyInfo.SetValue(command, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                    }
                    else
                        _logger.LogTrace($"Field '{key}' not found on type '{type.Name}'");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error mapping value {key} to {type.Name}");
                }
            }
        }

        private string ExtractValue(string[] parts, PropertyInfo propertyInfo)
        {
            var value = "";

            switch (parts.Length)
            {
                case 1 when propertyInfo.PropertyType == typeof(bool):
                    value = "True"; // "cmd -arg" is same as "cmd -arg=True"
                    break;
                case 1:
                    value = "";
                    break;
                default:
                    value = parts[1];
                    break;
            }

            return value;
        }
    }
}