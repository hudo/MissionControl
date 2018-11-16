using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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
        
        private readonly char[] _separators = new[] { '=',':' };

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

            if (!(Activator.CreateInstance(registration.Value.Type) is CliCommand command))
                throw new Exception("Command needs to inherit CliCommand");

            command.CorrelationId = request.ClientId;

            ParseArguments(request, registration.Value.Type, command);

            return command;
        }

        private void ParseArguments(Request request, Type type, CliCommand command)
        { 
            var hasHelpArg = false;
            var usedProps = new List<PropertyInfo>();
            var cmdProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => (propInfo: x,  attrib : x.GetCustomAttribute<CliArgAttribute>()))
                .ToArray();

            foreach (var arg in request.Args.Where(x => x.HasContent()))
            {
                var key = "";
                try
                {
                    // todo: ugly, refactor!
                    // implement simple tokenizer and parser
                    
                    var parts = arg.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
                    key = parts[0].TrimStart('-');

                    if (key.Equals("help", StringComparison.OrdinalIgnoreCase))
                        hasHelpArg = true;
                     
                    var cmdProp = cmdProps.FirstOrDefault(x => x.propInfo.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

                    if (cmdProp.propInfo != null)
                    {
                        if (cmdProp.attrib?.Skip == true)
                            continue;

                        var value = ExtractValue(parts, cmdProp.propInfo);

                        cmdProp.propInfo.SetValue(command, Convert.ChangeType(value, cmdProp.propInfo.PropertyType), null);
                        usedProps.Add(cmdProp.propInfo);
                    }
                    else
                        _logger.LogTrace($"Field [{key}] not found on type [{type.Name}]");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error mapping value of key [{key}] to [{type.Name}]");
                }
            }
    
            if (!hasHelpArg)
                CheckRequiredArguments(cmdProps, usedProps);
        }

        private static void CheckRequiredArguments((PropertyInfo propInfo, CliArgAttribute attrib)[] cmdProps, List<PropertyInfo> usedProps)
        {
            var missingArgs = cmdProps.Where(x => x.attrib?.Required == true && !usedProps.Contains(x.propInfo)).ToArray();

            if (missingArgs.Any())
            {
                throw new Exception($"Required arguments not populated: {string.Join(", ", missingArgs.Select(x => x.propInfo.Name.ToLower()))}");
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