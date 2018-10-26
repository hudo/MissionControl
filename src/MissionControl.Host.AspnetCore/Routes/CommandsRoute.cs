using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        private const string IdHeader = "mc.id";
        private const string ArgsHeader = "mc.args";
        private const string CmdUrlPrefix = "cmd/";

        private readonly IDispatcher _dispatcher;
        
        public CommandsRoute(IDispatcher dispatcher) : base("")
        {
            _dispatcher = dispatcher;
        }

        public override bool Match(string reqUri, string method)
        {
            return reqUri.StartsWith(CmdUrlPrefix) && method.Equals("post", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get CLI command info from request, eg:
        /// /mc/command-name
        /// Pull command args from request header 
        /// </summary>
        public override async Task Handle(string reqUri, HttpRequest request, HttpResponse response)
        {
            var validationResult = ValidateRequest(request);
            if (!validationResult.isValid)
            {
                response.StatusCode = 417;
                await response.WriteAsync(Json(validationResult.errors));
                return;
            }
            
            var req = BuildRequest(reqUri, request);

            var cliResponse = await _dispatcher.Invoke(req);
            
            // very basic rendering, refactor this
            var type = cliResponse.GetType().Name.Replace("Response", "").ToLower();
            response.StatusCode = (int)cliResponse.StatusCode;
            
            await response.WriteAsync(Json(new
            {
                type,
                content = cliResponse.Content,
                commandId = req.ClientId
            })); 
        }

        private static Request BuildRequest(string reqUri, HttpRequest request)
        {
            var req = new Request();

            // todo: what if clientId is not in header?
            req.ClientId = request.Headers[IdHeader].FirstOrDefault();
            req.Command = reqUri.Replace(CmdUrlPrefix, "").Trim('/'); // todo: ugly

            if (request.Headers.TryGetValue(ArgsHeader, out var values))
            {
                req.Args = values[0].Split(';');
            }

            return req;
        }

        private static readonly Regex CommandNameValidator = new Regex("(.*)(/cmd/)([a-z-_]{1,50})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        private (bool isValid, List<string> errors) ValidateRequest(HttpRequest request)
        {
            var errors = new List<string>();

            if (!request.Headers.ContainsKey(IdHeader))
            {
                errors.Add("'mc.id' console window identifier missing");
            }

            if (!CommandNameValidator.IsMatch(request.Path.ToString()))
            {
                errors.Add($"Error parsing command name: {request.Path.ToString()}");
            }
            
            return (!errors.Any(), errors);
        }

        private static string Json(object obj) => JsonConvert.SerializeObject(obj, JsonSettings);
        
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
        };
    }
}