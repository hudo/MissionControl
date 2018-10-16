using System;
using System.Net;

namespace MissionControl.Host.Core.Responses
{
    public abstract class CliResponse
    {
        protected CliResponse()
        {
            Id = Guid.NewGuid();
            StatusCode = HttpStatusCode.OK;
        }
        
        public Guid Id { get; }

        public string Content { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}