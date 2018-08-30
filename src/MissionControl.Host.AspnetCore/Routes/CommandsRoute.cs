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
        private static string _clientIdHeader = "mc.clientid";
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
            var req = new Request();
            var args = new List<(string key, string value)>();

            // todo: what if clientId is not in header?
            req.ClientId = request.Headers["mc.clientid"].FirstOrDefault();
            
            req.Command = reqUri.Replace("cmd/", "").Trim('/'); // todo: ugly, remove "cmd/" 

            if (request.Headers.TryGetValue(_argsHeader, out var values))
            {
                args.AddRange(values[0]
                    .Split(';')
                    .Select(x =>
                    {
                        var arg = x.Split('#');
                        return (key: arg[0], value: arg[1]);
                    }));
                
            }
            
            // flow:
            // - dispatcher (parse request, creates ConHost)
            // - conHost (finds handler, internal commands queue)
            // - command handler

            var cliResponse = await _dispatcher.Invoke(req);

            await response.WriteAsync(JsonConvert.SerializeObject(cliResponse)); // todo
        }
    }
}