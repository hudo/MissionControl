using System.Net.Http;
using System.Threading.Tasks;
using MissionControl.Host.AspnetCore.Tests.Integration.Infrastructure;
using MissionControl.Host.Core.Responses;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace MissionControl.Host.AspnetCore.Tests.Integration
{
    public class StandardCommandsTests : TestsBase<CustomWebAppFactory> 
    {
        public StandardCommandsTests(CustomWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task Standard_hello_world_command()
        {
            var client = GetClient();
            var content = new StringContent("");
            content.Headers.Add("mc.args", "name=batman");
            
            var response = await client.PostAsync("/mc/cmd/ping", content);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var textResponse = JsonConvert.DeserializeObject<TextResponse>(body);
            textResponse.ShouldNotBeNull();
            textResponse.Content.ShouldContain("batman");
        }
    }
}