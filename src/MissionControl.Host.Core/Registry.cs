using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Contracts.Pipeline;

namespace MissionControl.Host.Core
{
    public static class Registry
    {
        public static void RegisterServices(IServiceCollection services, Assembly[] assemblies)
        {
            services.AddSingleton<IDispatcher, Dispatcher>();
            services.AddTransient<IRequestParser, RequestParser>();
            services.AddSingleton<IConHostFactory, ConHostFactory>();

            services.AddScoped<ServiceFactory>(x => x.GetService);

            var catalog = new CommandTypesCatalog();

            services.AddSingleton<ICommandTypesCatalog, CommandTypesCatalog>(_ => catalog);
            
            catalog.DiscoverCommands(assemblies);
            
            services.Scan(x => x.FromAssemblies(assemblies)
                .AddClasses(cls => cls.AssignableTo(typeof(ICliCommandHandler<>))).AsImplementedInterfaces()
                .AddClasses(cls => cls.AssignableTo(typeof(IPipelineBehavior<>))).AsImplementedInterfaces()
                .AddClasses(cls => cls.AssignableTo(typeof(IPipelinePreBehavior<>))).AsImplementedInterfaces()
                .AddClasses(cls => cls.AssignableTo(typeof(IPipelinePostBehavior<>))).AsImplementedInterfaces());
        }
    }
}