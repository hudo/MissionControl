using System;

namespace MissionControl.Host.Core.Contracts
{
    public abstract class CliCommand
    {
        public string CorrelationId { get; set; }
    }
}
