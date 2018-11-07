using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Utilities;

namespace MissionControl.Host.Core
{
    internal class CommandTypesCatalog : ICommandTypesCatalog
    {
        public CommandRegistration[] RegisteredCommands { get; private set; }

        public void DiscoverCommands(Assembly[] assemblies)
        {
            var stopwatch = Stopwatch.StartNew();

            var registrations = new List<CommandRegistration>();

            var baseCommandType = typeof(CliCommand);

            var commandTypes = from assembly in assemblies
                from type in assembly.GetTypes()
                where type.IsClass && !type.IsAbstract && type.IsSubclassOf(baseCommandType)
                select type;

            foreach (var type in commandTypes)
            {
                var attribute = type.GetCustomAttribute<CliCommandAttribute>();
                if (attribute != null)
                {
                    registrations.Add(new CommandRegistration { Name = attribute.CommandText.ToLower(), Type = type, Attribute = attribute});
                }
                else
                {
                    Console.WriteLine($"warning: CliCommand {type.Name} is missing CliCommandAttribute!");
                }
            }

            RegisteredCommands = registrations.ToArray();
            
            stopwatch.Stop();
            
            Console.WriteLine($"Discovered {registrations.Count} CLI commands in assemblies {string.Join(", ", assemblies.Select(x => x.GetName().Name))}");
            Console.WriteLine($"Assembly scanning done in {stopwatch.ElapsedMilliseconds}ms");
        }
    }

    public class CommandRegistration
    {
        public string Name;
        public Type Type;
        public CliCommandAttribute Attribute;
    }

    internal static class CommandTypeCatalogExtensions
    {
        public static Maybe<CommandRegistration> FindCommandByName(this ICommandTypesCatalog catalog, string name) =>
            catalog.RegisteredCommands.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}