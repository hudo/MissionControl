using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MissionControl.Host.AspnetCore.Routes;

namespace MissionControl.Host.AspnetCore
{
    public class MissionControlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly McOptions _options;

        private readonly PathString _urlPrefix;

        private readonly List<Route> _routes;

        public MissionControlMiddleware(RequestDelegate next, McOptions options)
        {
            _next = next;

            if (options.Authentication == null)
                throw new ArgumentNullException(nameof(options.Authentication), "Request authentication nor provided");

            _options = options;
            _urlPrefix = new PathString(options.Url);

            var assembly = this.GetType().GetTypeInfo().Assembly;
            
            _routes = new List<Route>
            {
                new StaticContentRoute(assembly),
                new DefaultIndexRoute(assembly)
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(_urlPrefix, out var suffixPath))
            {
                if (!_options.Authentication(context.Request))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }

                context.Response.ContentType = "text/html";

                var uri = suffixPath.Value.Trim('/').ToLower();
                var route = _routes.FirstOrDefault(x => x.Match(uri));

                if (route != null)
                {
                    context.Response.StatusCode = 200;
                    await route.Hadle(uri, context.Response);
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}