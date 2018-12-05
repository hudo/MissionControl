using System.Net;

namespace MissionControl.Host.Core.Responses
{
    public sealed class ErrorResponse : CliResponse
    {
        public ErrorResponse(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            Content = message;
            StatusCode = statusCode;
            IsDone = true;
        }

        public override string Type => "error";
    }
}