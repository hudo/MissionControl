namespace MissionControl.Host.Core.Contracts
{
    public abstract class CliCommand
    {
        [CliArg(skip: true)]
        public string CorrelationId { get; set; }

        /// <summary>
        /// Displays help for invoked command
        /// </summary>
        [CliArg(help: "Display command help")]
        public bool Help { get; set; }
    }
}
