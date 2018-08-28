using Microsoft.Extensions.DependencyInjection;

namespace MissionControl.Host.Core
{
    public static class Registry
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IDispatcher, Dispatcher>();
            services.AddTransient<IRequestParser, RequestParser>();
            services.AddSingleton<IConHostFactory, ConHostFactory>();
            services.AddSingleton<ICommandTypesCatalog, CommandTypesCatalog>();
        }
    }
}