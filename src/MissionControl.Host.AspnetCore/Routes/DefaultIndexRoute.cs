using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MissionControl.Host.AspnetCore.Routes
{
    /// <summary>
    /// Serves index.html
    /// </summary>
    public class DefaultIndexRoute : Route
    {
        private readonly Stream _content;

        /// <summary>
        /// Serves index.html for GET /mc request 
        /// </summary>
        public DefaultIndexRoute(Assembly assembly) : base("", "get")
        {
            _content = assembly.GetManifestResourceStream("MissionControl.Host.AspnetCore.Content.index.html");
        }

        public override async Task Handle(string reqUri, HttpRequest httpRequest, HttpResponse httpResponse)
        {
            _content.Seek(0, SeekOrigin.Begin);

            await _content.CopyToAsync(httpResponse.Body);
        }
    }
}