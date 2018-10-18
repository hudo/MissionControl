namespace MissionControl.Host.Core.Contracts.StandardCommands
{
    [CliCommand("ping")]
    internal class PingCommand : CliCommand
    {
        [CliArg(required: true, help: "Say your name")]
        public string Name { get; set; }
    }
}