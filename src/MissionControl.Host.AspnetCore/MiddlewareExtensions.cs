using System;
using Microsoft.AspNetCore.Builder;

namespace MissionControl.Host.AspnetCore
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMissingControl(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MissionControlMiddleware>();
        }
    }
}