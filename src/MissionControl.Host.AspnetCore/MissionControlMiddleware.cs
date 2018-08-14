using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MissionControl.Host.AspnetCore
{
    public class MissionControlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly McOptions _options;

        public MissionControlMiddleware(RequestDelegate next, McOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);
        }
    }
}