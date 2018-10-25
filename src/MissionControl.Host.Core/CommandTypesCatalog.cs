using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Contracts.Pipeline;
using MissionControl.Host.Core.Utilities;

namespace MissionControl.Host.Core
{
    internal class CommandTypesCatalog : ICommandTypesCatalog
    {
        public CommandRegistration[] RegisteredCommands { get; private set; }

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
            }

            RegisteredCommands = registrations.ToArray();

            Console.WriteLine($"Discovered {registrations.Count} CLI commands in assemblies {string.Join(", ", assemblies.Select(x => x.GetName().Name))}");
        }

        private void RegisterHandlers(IServiceCollection services, Assembly[] assemblies)
        {
            // move outside

            services.Scan(x => x.FromAssemblies(assemblies)
                .AddClasses(cls => cls.AssignableTo(typeof(ICliCommandHandler<>))).AsImplementedInterfaces()
                .AddClasses(cls => cls.AssignableTo(typeof(IPipelineBehavior<>))).AsImplementedInterfaces()
                .AddClasses(cls => cls.AssignableTo(typeof(IPipelinePreBehavior<>))).AsImplementedInterfaces()
                .AddClasses(cls => cls.AssignableTo(typeof(IPipelinePostBehavior<>))).AsImplementedInterfaces());
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