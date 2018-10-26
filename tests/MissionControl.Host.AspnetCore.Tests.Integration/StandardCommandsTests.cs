using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MissionControl.Host.AspnetCore.Tests.Integration.Infrastructure;
using MissionControl.Host.Core.Responses;
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
            var response = await Post<TextResponse>("/mc/cmd/ping", "name=batman");

            response.HttpResponse.EnsureSuccessStatusCode();

            response.Item.ShouldNotBeNull();
            response.Item.Content.ShouldContain("batman");
        }

        [Fact]
        public async Task List_commands_command()
        {
            var response = await Post<TextResponse>("/mc/cmd/list-commands");

            response.HttpResponse.EnsureSuccessStatusCode();

            response.Item.Content.StartsWith("Registered commands", StringComparison.OrdinalIgnoreCase).ShouldBeTrue();
            response.Item.Content.Contains("list-commands", StringComparison.OrdinalIgnoreCase).ShouldBeTrue();
        }

        [Fact]
        public async Task Help_arg_returns_command_description()
        {
            var response = await Post<TextResponse>("/mc/cmd/ping", "help");

            response.HttpResponse.EnsureSuccessStatusCode();

            response.Item.Content.StartsWith("Description of command").ShouldBeTrue();
            response.Item.Content.Contains("CorrelationId", StringComparison.OrdinalIgnoreCase).ShouldBeFalse();
        }

        [Fact]
        public async Task No_command_name_in_path_returns_error()
        {
            var response = await Post<List<string>>("/mc/cmd/");
            
            response.HttpResponse.StatusCode.ShouldBe(HttpStatusCode.ExpectationFailed);
        }
    }
}