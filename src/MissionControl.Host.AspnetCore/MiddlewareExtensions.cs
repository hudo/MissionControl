using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MissionControl.Host.Core;

namespace MissionControl.Host.AspnetCore
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMissingControl(this IApplicationBuilder builder, Action<McOptions> configuration = null)
        {
            var options = new McOptions();

            configuration?.Invoke(options);

            if (options.Assemblies == null)
                options.Assemblies = new[] {Assembly.GetCallingAssembly()};

            return builder.UseMiddleware<MissionControlMiddleware>(options);
        }

        public static void AddMissionControl(this IServiceCollection services)
        {
            Registry.RegisterServices(services);
        }
    }
}