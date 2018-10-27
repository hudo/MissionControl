using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MissionControl.Host.AspnetCore.Routes
{
    public abstract class Route
    {
        protected string _path;
        private readonly string[] _methods;

        protected Route(string path, params string[] methods)
        {
            _path = path;
            _methods = methods;

            if (_methods == null || _methods.Length == 0)
                _methods = new[] {"get"};
        }

        /// <summary>
        /// Check if route matches current request
        /// </summary>
        /// <param name="reqUri">URL part after /mc</param>
        /// <returns>True if route match</returns>
        public virtual bool Match(string reqUri, string method) // todo: refactor method
        {
            return _path == reqUri && _methods.Contains(method);
        }

        /// <summary>
        /// Handle request
        /// </summary>
        /// <param name="reqUri">URL part after /mc</param>
        public abstract Task Handle(string reqUri, HttpRequest httpRequest, HttpResponse httpResponse); // todo: revisit this
    }
}