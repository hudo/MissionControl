using System;

namespace MissionControl.Host.Core
{
    public /*abstract*/ class CliCommand
    {
        public CliCommand()
        {
            Id = Guid.NewGuid(); 
        }
        
        public string Input { get; set; }
        
        public Guid Id { get; }
    }
}
