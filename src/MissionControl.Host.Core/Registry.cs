using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MissionControl.Host.Core.Contracts;

namespace MissionControl.Host.Core
{
    public static class Registry
    {
        public static void RegisterServices(IServiceCollection services, Assembly[] assemblies)
        {
            services.AddSingleton<IDispatcher, Dispatcher>();
            services.AddTransient<IRequestParser, RequestParser>();
            services.AddSingleton<IConHostFactory, ConHostFactory>();
            services.AddSingleton<ICommandTypesCatalog, CommandTypesCatalog>();
            
            var handler = typeof(ICliCommandHandler<>);

            var types =
                from ass in assemblies
                from type in ass.GetTypes()
                from i in type.GetInterfaces()
                where i.IsGenericType && handler.IsAssignableFrom(i.GetGenericTypeDefinition())
                select type;

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces().FirstOrDefault();
                services.AddTransient(interfaces, type);
            }
        }
    }
}