using System;

namespace MissionControl.Host.Core.Contracts
{
    public abstract class CliCommand
    {
        public string CorrelationId { get; set; }

        /// <summary>
        /// Displays help for invoked command
        /// </summary>
        public bool Help { get; set; }
    }
}
