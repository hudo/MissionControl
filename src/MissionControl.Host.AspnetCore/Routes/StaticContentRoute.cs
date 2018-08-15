using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MissionControl.Host.AspnetCore.Routes
{
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

        public override bool Match(string reqUri)
        {
            return !string.IsNullOrEmpty(reqUri) && _contentTypes.ContainsKey(GetExtension(reqUri));
        }

        public override async Task Hadle(string reqUri, HttpResponse response)
        {
            var stream = _assembly.GetManifestResourceStream("MissionControl.Host.AspnetCore.Content." + reqUri);

            if (stream == null)
            {
                response.StatusCode = 404;
                return;
            }

            var ext = GetExtension(reqUri);

            response.ContentType = _contentTypes.ContainsKey(ext)
                ? _contentTypes[ext]
                : "text/plain";

            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(response.Body);
        }

        private static string GetExtension(string reqUri)
        {
            var pos = reqUri.LastIndexOf(".", StringComparison.Ordinal);

            return pos < 0 ? string.Empty : reqUri.Substring(pos);
        }
    }
}