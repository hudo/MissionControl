using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace MissionControl.Host.Core
{
    internal interface ICommandTypesCatalog
    {
        void ScanAssemblies(Assembly[] assemblies);

        (Type type, CommandTextAttribute attributes) GetTypeByCommandName(string name);
    }

    
    internal class CommandTypesCatalog : ICommandTypesCatalog
    {
        private readonly ILogger<CommandTypesCatalog> _logger;
        
        private readonly Dictionary<string, (Type type, CommandTextAttribute attribute)> _commandNames = new Dictionary<string, (Type type, CommandTextAttribute attribute)>();

        public CommandTypesCatalog(ILogger<CommandTypesCatalog> logger)
        {
            _logger = logger;
        }
        
        public void ScanAssemblies(Assembly[] assemblies)
        {
            var baseCommandType = typeof(CliCommand);

            var commandTypes = from assembly in assemblies
                from type in assembly.GetTypes()
                where type.IsClass && !type.IsAbstract && type.IsSubclassOf(baseCommandType)
                select type;

            var count = 0;
            
            foreach (var type in commandTypes)
            {
                var attribute = type.GetCustomAttribute<CommandTextAttribute>();

                if (attribute != null)
                {
                    _commandNames.Add(attribute.CommandText.ToLower(), (type, attribute));
                    count += 1;
                }
            }

            _logger.LogTrace($"Discovered {count} CLI commands in assemblies {string.Join(", ", assemblies.Select(x => x.FullName))}");
        }

        public (Type type, CommandTextAttribute attributes) GetTypeByCommandName(string name)
        {
            if (_commandNames.ContainsKey(name.ToLower()))
            {
                var mapping = _commandNames[name];
                return (mapping.type, mapping.attribute);
            }

            return (null, null);
        }
    }
}