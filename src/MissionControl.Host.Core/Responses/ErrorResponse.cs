using System.Net;

namespace MissionControl.Host.Core.Responses
{
    public sealed class ErrorResponse : CliResponse
    {
        public ErrorResponse(string message)
        {
            Content = message;
            StatusCode = HttpStatusCode.BadRequest;
            IsDone = true;
        }

        public override string Type => "error";
    }
}