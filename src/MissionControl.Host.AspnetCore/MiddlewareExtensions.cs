using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MissionControl.Host.Core;

namespace MissionControl.Host.AspnetCore
{
    public static class MiddlewareExtensions
    {
        private static Assembly[] _assemblies;
        
        public static IApplicationBuilder UseMissingControl(this IApplicationBuilder builder, Action<McOptions> configuration = null)
        {
            var options = new McOptions();

            configuration?.Invoke(options);

            return builder.UseMiddleware<MissionControlMiddleware>(options, _assemblies);
        }

        /// <summary>
        /// Register services into container
        /// </summary>
        /// <param name="assemblies">Assemblies containing commands and handlers</param>
        public static void AddMissionControl(this IServiceCollection services, params Assembly[] assemblies)
        {
            _assemblies = assemblies.Length > 0 ? assemblies : new[] {Assembly.GetCallingAssembly()};
            Registry.RegisterServices(services, _assemblies);
        }
    }
}