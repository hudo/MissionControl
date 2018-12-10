using System.Collections.Async;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MissionControl.Host.Core.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MissionControl.Host.AspnetCore
{
    public delegate Task OutputWriter(CliResponse cliResponse, HttpResponse httpResponse);
    
    internal static class HttpOutputWriter
    {
        public static async Task Write(CliResponse cliResponse, HttpResponse httpResponse)
        {
            if (cliResponse is MultipleResponses multipleResponses)
            {
                await multipleResponses.Responses().ForEachAsync(async payload =>
                {
                    await httpResponse.WriteAsync(Json(new StreamingResponse(payload, false)));
                    await httpResponse.Body.FlushAsync();
                });
            }
            else
            {
                await httpResponse.WriteAsync(Json(cliResponse));
            }
        }
        
        private static string Json(object obj) => JsonConvert.SerializeObject(obj, JsonSettings);
        
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        
        class StreamingResponse : CliResponse
        {
            public StreamingResponse(string content, bool isDone)
            {
                Type = "multipart";
                IsDone = isDone;
                Content = content;
            }
            
            public override string Type { get; } 
        }
    }
}