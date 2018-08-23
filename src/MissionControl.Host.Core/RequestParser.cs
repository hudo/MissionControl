namespace MissionControl.Host.Core
{
    internal class RequestParser : IRequestParser
    {
        public CliCommand Parse(Request request)
        {
            return null;
        }
    }
    
    // deserialize request to DTO props
    internal interface IRequestParser
    {
        CliCommand Parse(Request request);
    }
}