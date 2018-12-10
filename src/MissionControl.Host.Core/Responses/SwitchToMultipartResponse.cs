namespace MissionControl.Host.Core.Responses
{
    internal class SwitchToMultipartResponse : CliResponse
    {
        public SwitchToMultipartResponse()
        {
            IsDone = true;
        }

        public override string Type { get; } = "multipart-switch";
    }
}