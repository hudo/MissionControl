using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MissionControl.Host.AspnetCore.Routes
{
    public abstract class Route
    {
        protected string _path;

        protected Route(string path)
        {
            _path = path;
        }

        public virtual bool Match(string reqUri)
        {
            return _path == reqUri;
        }

        public abstract Task Hadle(string reqUri, HttpResponse response);
    }
}