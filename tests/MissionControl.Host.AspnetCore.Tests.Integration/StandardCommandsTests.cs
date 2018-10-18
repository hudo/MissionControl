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
            var response = await Post("/mc/cmd/ping", "name=batman");

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var textResponse = JsonConvert.DeserializeObject<TextResponse>(body);
            textResponse.ShouldNotBeNull();
            textResponse.Content.ShouldContain("batman");
        }

        [Fact]
        public async Task List_commands_command()
        {
            var response = await Post("/mc/cmd/list-commands");

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
        }
    }
}