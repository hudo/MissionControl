namespace MissionControl.Host.Core.Responses
{
    public sealed class TextResponse : CliResponse
    {
        public TextResponse(string content)
        {
            Content = content;
        }
        
        public string Content { get; set; }
    }
}