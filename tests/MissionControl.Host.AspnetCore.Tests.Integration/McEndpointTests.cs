using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MissionControl.Host.AspnetCore.Tests.Integration.Infrastructure;
using Shouldly;
using Xunit;

namespace MissionControl.Host.AspnetCore.Tests.Integration
{
    public class McEndpointTests : IClassFixture<CustomWebAppFactory>
    {
        private readonly CustomWebAppFactory _appFactory;

        public McEndpointTests(CustomWebAppFactory appFactory)
        {
            _appFactory = appFactory;
        }
        
        [Fact]
        public async Task Root_returns_success_code()
        {
            var httpClient = _appFactory.CreateClient();

            var response = await httpClient.GetAsync("/mc/");

            response.EnsureSuccessStatusCode();
            
            var body = await response.Content.ReadAsStringAsync();
            body.ShouldContain("<title>Mission Control CLI Terminal</title>");
            
            response.Content.Headers.ContentType.ToString().ShouldBe("text/html");
        }
    }
    
}