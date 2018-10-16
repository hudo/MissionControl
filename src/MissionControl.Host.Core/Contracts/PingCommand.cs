namespace MissionControl.Host.Core.Contracts
{
    [CliCommand("ping")]
    public class PingCommand : CliCommand
    {
        [CliArg(required: true, help: "Say your name")]
        public string Name { get; set; }
    }
}