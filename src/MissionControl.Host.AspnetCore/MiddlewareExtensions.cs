using System;
using Microsoft.AspNetCore.Builder;

namespace MissionControl.Host.AspnetCore
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMissingControl(this IApplicationBuilder builder, Action<McOptions> configuration = null)
        {
            var options = new McOptions();

            configuration?.Invoke(options);

            return builder.UseMiddleware<MissionControlMiddleware>(options);
        }
    }
}