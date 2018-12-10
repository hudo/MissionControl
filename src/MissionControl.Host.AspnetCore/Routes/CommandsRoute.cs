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
        private readonly OutputWriter _outputWriter;

        public CommandsRoute(IDispatcher dispatcher, OutputWriter outputWriter) : base("")
        {
            _dispatcher = dispatcher;
            _outputWriter = outputWriter;
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
                cliResponse = new ErrorResponse(
                    string.Join(", ", validationResult.errors), 
                    HttpStatusCode.ExpectationFailed);
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

            await _outputWriter(cliResponse, httpResponse);
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
                req.Args = args.Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries);
            }

            return req;
        }

        private static readonly Regex CommandNameValidator = new Regex("(.*)(/cmd/)([a-z-_1-9]{1,50})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
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
        
    }
}