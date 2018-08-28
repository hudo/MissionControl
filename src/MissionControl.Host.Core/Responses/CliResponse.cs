using System;

namespace MissionControl.Host.Core.Responses
{
    public abstract class CliResponse
    {
        protected CliResponse()
        {
            Id = Guid.NewGuid();    
        }
        
        public Guid Id { get; }
        public Guid RequestId { get; set; }
    }
}