using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MissionControl.Host.Core.Contracts;

namespace MissionControl.Host.Core
{
    internal class CommandTypesCatalog : ICommandTypesCatalog
    {
        private readonly Dictionary<string, (Type type, CliCommandAttribute attribute)> _commandNames = new Dictionary<string, (Type type, CliCommandAttribute attribute)>();
        
        public void ScanAssemblies(Assembly[] assemblies, IServiceCollection services)
        {
            var stopwatch = Stopwatch.StartNew();

            DiscoverCommands(assemblies);

            RegisterHandlers(services, assemblies);

            stopwatch.Stop();
            Console.WriteLine($"Assembly scanning done in {stopwatch.ElapsedMilliseconds}ms");
        }

        private void DiscoverCommands(Assembly[] assemblies)
        {
            var baseCommandType = typeof(CliCommand);

            var commandTypes = from assembly in assemblies
                from type in assembly.GetTypes()
                where type.IsClass && !type.IsAbstract && type.IsSubclassOf(baseCommandType)
                select type;

            var count = 0;
            
            foreach (var type in commandTypes)
            {
                var attribute = type.GetCustomAttribute<CliCommandAttribute>();
                if (attribute != null)
                {
                    _commandNames.Add(attribute.CommandText.ToLower(), (type, attribute));
                    count += 1;
                }
            }

            Console.WriteLine($"Discovered {count} CLI commands in assemblies {string.Join(", ", assemblies.Select(x => x.GetName().Name))}");
        }

        private void RegisterHandlers(IServiceCollection services, Assembly[] assemblies)
        {
            var handler = typeof(ICliCommandHandler<>);

            var handlerTypes =
                from ass in assemblies
                from type in ass.GetTypes()
                from i in type.GetInterfaces()
                where i.IsGenericType && handler.IsAssignableFrom(i.GetGenericTypeDefinition())
                select type;

            foreach (var handlerType in handlerTypes)
            {
                var handlerInterface = handlerType.GetInterfaces().FirstOrDefault();
                services.AddTransient(handlerInterface, handlerType);
            }
        }

        public (Type type, CliCommandAttribute attributes) GetTypeByCommandName(string name)
        {
            if (_commandNames.ContainsKey(name.ToLower()))
            {
                var mapping = _commandNames[name];
                return (mapping.type, mapping.attribute);
            }

            return (null, null); // any better way of returning nullable tuple? 
        }

        public string[] RegisteredCommands => _commandNames.Select(x => x.Key).ToArray();
    }
}