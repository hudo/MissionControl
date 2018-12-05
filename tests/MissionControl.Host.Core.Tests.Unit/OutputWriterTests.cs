using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MissionControl.Host.AspnetCore;
using MissionControl.Host.Core.Responses;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace MissionControl.Host.Core.Tests.Unit
{
    public class OutputWriterTests
    {
        [Fact]
        public async Task Write_multiple_responses()
        {
            var responses = new List<StreamResponse>();

            var response = new Mock<HttpResponse>();
            response
                .Setup(x => x.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Callback((byte[] payload, int _1, int _2, CancellationToken _3) =>
                {
                    responses.Add(JsonConvert.DeserializeObject<StreamResponse>(Encoding.UTF8.GetString(payload)));
                })
                .Returns(Task.CompletedTask)
                .Verifiable();
            
            CliResponse cliResponse = new MultipleResponses(async yield =>
            {
                await yield.ReturnAsync("1");
                await yield.ReturnAsync("2");
            });

            await HttpOutputWriter.Write(cliResponse, response.Object);
            
            response.Verify(x => x.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), 
                Times.Exactly(3));
            
            responses.Count.ShouldBe(3);
            responses[0].IsDone.ShouldBe(false);
            responses[1].IsDone.ShouldBe(false);
            responses[2].IsDone.ShouldBe(true);
        }

        [Fact]
        public async Task Write_single_response()
        {
            var responses = new List<CliResponse>();

            var response = new Mock<HttpResponse>();
            response
                .Setup(x => x.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Callback((byte[] payload, int _1, int _2, CancellationToken _3) =>
                {
                    responses.Add(JsonConvert.DeserializeObject<TextResponse>(Encoding.UTF8.GetString(payload)));
                })
                .Returns(Task.CompletedTask)
                .Verifiable();

            await HttpOutputWriter.Write(new TextResponse("a"), response.Object);
            
            response.Verify(x => x.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), 
                Times.Once);
            
            responses[0].IsDone.ShouldBe(true);
            responses[0].Content.ShouldBe("a");
        }

        class StreamResponse
        {
            public string Type { get; set; }
            public string Content { get; set; }
            public bool IsDone { get; set; }
        }
    }
}