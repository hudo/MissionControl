using System;
using System.Net;

namespace MissionControl.Host.Core.Responses
{
    public abstract class CliResponse
    {
        protected CliResponse()
        {
            StatusCode = HttpStatusCode.OK;
        }
        
        public string Content { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Command type name. Client terminal app uses this to understand type of command, like text or error.
        /// </summary>
        public abstract string Type { get; }

        /// <summary>
        /// Clients terminal should keep polling for more results
        /// </summary>
        public bool IsDone { get; protected set; } = true;
    }
}