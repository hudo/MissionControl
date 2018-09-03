using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MissionControl.Host.Core;
using Newtonsoft.Json;

namespace MissionControl.Host.AspnetCore.Routes
{
    /// <summary>
    /// Serves all CLI requests
    /// </summary>
    public class CommandsRoute : Route
    {
        private readonly IDispatcher _dispatcher;
        private static string _idHeader = "mc.id";
        private static string _argsHeader = "mc.args"; 
        
        public CommandsRoute(IDispatcher dispatcher) : base("")
        {
            _dispatcher = dispatcher;
        }

        public override bool Match(string reqUri, string method)
        {
            return reqUri.StartsWith("cmd/"); //&& method == "post";
        }

        /// <summary>
        /// Get CLI command info from request, eg:
        /// /mc/command-name
        /// Pull command args from request header 
        /// </summary>
        public override async Task Handle(string reqUri, HttpRequest request, HttpResponse response)
        {
            var req = BuildRequest(reqUri, request);

            var cliResponse = await _dispatcher.Invoke(req);
            
            // very basic rendering, refactor this

            var type = cliResponse.GetType().Name.Replace("Response", "").ToLower();
            
            await response.WriteAsync(JsonConvert.SerializeObject(new
            {
                type,
                content = cliResponse.Content,
                commandId = req.CorrelationId
            })); 
        }

        private static Request BuildRequest(string reqUri, HttpRequest request)
        {
            var req = new Request();

            // todo: what if clientId is not in header?
            req.CorrelationId = request.Headers[_idHeader].FirstOrDefault();
            req.Command = reqUri.Replace("cmd/", "").Trim('/'); // todo: ugly

            if (request.Headers.TryGetValue(_argsHeader, out var values))
            {
                req.Args = values[0].Split(';');
            }

            return req;
        }
    }
}