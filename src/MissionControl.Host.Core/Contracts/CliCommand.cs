using System;

namespace MissionControl.Host.Core.Contracts
{
    public abstract class CliCommand
    {
        protected CliCommand()
        {
            Id = Guid.NewGuid(); 
        }
        
        public Guid Id { get; }
    }
}
