using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MissionControl.Host.AspnetCore.Routes
{
    /// <summary>
    /// Serve static files from internal assembly resources
    /// </summary>
    public class StaticContentRoute : Route
    {
        private readonly Assembly _assembly;

        private readonly Dictionary<string, string> _contentTypes = new Dictionary<string, string>
        {
            { ".js", "application/javascript" },
            { ".css", "text/css" },
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".gif", "image/gif" },
        };

        public StaticContentRoute(Assembly assembly) : base("")
        {
            _assembly = assembly;
        }

        public override bool Match(string reqUri, string method)
        {
            return !string.IsNullOrEmpty(reqUri) && _contentTypes.ContainsKey(GetExtension(reqUri)) && method == "get";
        }

        public override async Task Handle(string reqUri, HttpRequest httpRequest, HttpResponse httpResponse)
        {
            var stream = _assembly.GetManifestResourceStream("MissionControl.Host.AspnetCore.Content." + reqUri);

            if (stream == null)
            {
                httpResponse.StatusCode = 404;
                return;
            }

            var ext = GetExtension(reqUri);

            httpResponse.ContentType = _contentTypes.ContainsKey(ext)
                ? _contentTypes[ext]
                : "text/plain";

            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(httpResponse.Body);
        }

        private static string GetExtension(string reqUri)
        {
            var pos = reqUri.LastIndexOf(".", StringComparison.Ordinal);

            return pos < 0 ? string.Empty : reqUri.Substring(pos);
        }
    }
}