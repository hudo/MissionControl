namespace MissionControl.Host.Core.Responses
{
    public sealed class ErrorResponse : CliResponse
    {
        public ErrorResponse(string message)
        {
            Content = message;
        }
        
        public string Content { get; set; }
    }
}