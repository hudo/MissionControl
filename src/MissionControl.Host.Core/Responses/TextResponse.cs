namespace MissionControl.Host.Core.Responses
{
    public sealed class TextResponse : CliResponse
    {
        public TextResponse(string content)
        {
            Content = content;
        }

        public override string Type => "text";
    }
}