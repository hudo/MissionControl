namespace MissionControl.Host.Core.Contracts
{
    public abstract class CliCommand
    {
        [CliArg(skip: true)]
        public string ClientId { get; set; }

        /// <summary>
        /// Displays help for invoked command
        /// </summary>
        [CliArg(help: "Display command help")]
        public bool Help { get; set; }
    }
}
