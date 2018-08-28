namespace MissionControl.Host.Core.Contracts
{
    [CliCommand("hello-world")]
    public class HelloWorldCommand : CliCommand
    {
        public string Name { get; set; }
    }
}