using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MissionControl.Host.Core;
using MissionControl.Host.Core.Responses;
using MissionControl.Host.Core.Utilities;
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
        private const string CmdUrlPrefix = "cmd";

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
        public override async Task Handle(string reqUri, HttpRequest httpRequest, HttpResponse httpResponse)
        {
            var validationResult = ValidateRequest(httpRequest);

            var clientId = httpRequest.Headers[IdHeader];
            CliResponse cliResponse;

            if (!validationResult.isValid)
            {
                cliResponse = new ErrorResponse(string.Join(", ", validationResult.errors))
                {
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
            else
            {
                try
                {
                    var request = BuildRequest(reqUri, httpRequest);
                    cliResponse = await _dispatcher.Invoke(request);
                }
                catch (Exception e)
                {
                    cliResponse = new ErrorResponse(e.Unwrap().Message);
                }

            }

            httpResponse.StatusCode = (int)cliResponse.StatusCode;
            cliResponse.TerminalId = clientId;

            await httpResponse.WriteAsync(Json(cliResponse)); 
        }

        
        private static Request BuildRequest(string reqUri, HttpRequest request)
        {
            var req = new Request();

            // todo: what if clientId is not in header?
            req.ClientId = request.Headers[IdHeader];
            req.Command = reqUri.Replace(CmdUrlPrefix, "").Trim('/'); // todo: ugly

            var args = request.Headers[ArgsHeader].ToString();
            if (args.HasContent())
            {
                req.Args = args.Split(';');
            }

            return req;
        }

        private static readonly Regex CommandNameValidator = new Regex("(.*)(/cmd/)([a-z-_]{1,50})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        // todo: move to validator class
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