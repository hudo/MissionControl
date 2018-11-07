using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MissionControl.Host.AspnetCore.Routes;
using MissionControl.Host.Core;

namespace MissionControl.Host.AspnetCore
{
    public class MissionControlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly McOptions _options;
        private readonly ILogger<MissionControlMiddleware> _logger;

        private readonly PathString _urlPrefix;

        private readonly List<Route> _routes;

        public MissionControlMiddleware(RequestDelegate next, 
            McOptions options,
            IDispatcher dispatcher, 
            ILogger<MissionControlMiddleware> logger)
        {
            _next = next;

            if (options.Authentication == null)
                throw new ArgumentNullException(nameof(options.Authentication), "Request authentication nor provided");

            _options = options;
            _logger = logger;
            _urlPrefix = new PathString(options.Url);


            var assembly = this.GetType().GetTypeInfo().Assembly;

            _routes = new List<Route>
            {
                new StaticContentRoute(assembly),
                new DefaultIndexRoute(assembly),
                new CommandsRoute(dispatcher)
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(_urlPrefix, out var suffixPath))
            {
                await HandleCommand(context, suffixPath);
            }
            else
            {
                await _next(context);
            }
        }

        private async Task HandleCommand(HttpContext context, PathString suffixPath)
        {
            if (!_options.Authentication(context.Request))
            {
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                _logger.LogDebug($"Request not authorized: {context.Request.Path}");
                return;
            }

            context.Response.ContentType = "text/html";

            var uri = suffixPath.Value.Trim('/').ToLower();
            var route = _routes.FirstOrDefault(x => x.Match(uri, context.Request.Method.ToLower()));

            if (route != null)
            {
                _logger.LogTrace($"Route [{route.GetType().Name}] matched for request {context.Request.Path}");

                await route.Handle(uri, context.Request, context.Response);
            }
            else
            {
                _logger.LogDebug($"Route not found for request {context.Request.Path}");

                context.Response.StatusCode = 404;
            }
        }
    }
}