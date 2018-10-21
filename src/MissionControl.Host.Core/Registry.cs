using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

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
            
            catalog.ScanAssemblies(assemblies, services);
        }
    }
}