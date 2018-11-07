using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Pipeline;

namespace MissionControl.Host.Core
{
    public static class Registry
    {
        public static void RegisterServices(IServiceCollection services, Assembly[] assemblies)
        {
            services.AddSingleton<IDispatcher, Dispatcher>();
            services.AddTransient<IRequestParser, RequestParser>();
            services.AddSingleton<IConHostFactory, ConHostFactory>();
            
            services.AddSingleton<ServiceFactory>(x => x.GetService);
            
            var catalog = new CommandTypesCatalog();

            services.AddSingleton<ICommandTypesCatalog, CommandTypesCatalog>(_ => catalog);
            
            // scan in background to prevent slowing down app startup
            Task.Run(() => catalog.DiscoverCommands(assemblies));
            
            services.Scan(x => x.FromAssemblies(assemblies)
                .AddClasses(cls => cls.AssignableTo(typeof(ICliCommandHandler<>))).AsImplementedInterfaces()
                .AddClasses(cls => cls.AssignableTo(typeof(IPipelineBehavior<>))).AsImplementedInterfaces()
                .AddClasses(cls => cls.AssignableTo(typeof(IPipelinePreBehavior<>))).AsImplementedInterfaces()
                .AddClasses(cls => cls.AssignableTo(typeof(IPipelinePostBehavior<>))).AsImplementedInterfaces());
        }
    }
}