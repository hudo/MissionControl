using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MissionControl.Host.AspnetCore.Tests.Integration.Infrastructure;
using Shouldly;
using Xunit;

namespace MissionControl.Host.AspnetCore.Tests.Integration
{
    public class McEndpointTests : TestsBase<CustomWebAppFactory>
    {
        public McEndpointTests(CustomWebAppFactory appFactory) : base(appFactory)
        {
        }

        [Fact]
        public async Task Root_returns_success_code()
        {
            var response = await GetClient().GetAsync("/mc/");

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            body.ShouldContain("<title>Mission Control CLI Terminal</title>");
            response.Content.Headers.ContentType.ToString().ShouldBe("text/html");
        }

        [Fact]
        public async Task Get_static_file()
        {
            var response = await GetClient().GetAsync("/mc/styles.css");

            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType.ToString().ShouldBe("text/css");

            var body = await response.Content.ReadAsStringAsync();
            body.Length.ShouldBeGreaterThan(10);
            body.ShouldContain("body");
        }

        [Fact]
        public async Task Static_file_not_found_returns_404()
        {
            var response = await GetClient().GetAsync("/mc/foo.css");
            
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
    
}