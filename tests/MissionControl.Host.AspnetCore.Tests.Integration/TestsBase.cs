using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MissionControl.Host.AspnetCore.Tests.Integration.Infrastructure;
using MissionControl.Host.Core.Contracts;
using Xunit;

namespace MissionControl.Host.AspnetCore.Tests.Integration
{
    public abstract class TestsBase<T> : IClassFixture<T> where T : WebApplicationFactory<Startup>
    {
        protected readonly T Factory;

        protected TestsBase(T factory)
        {
            Factory = factory;
        }
        
        protected HttpClient GetClient() => Factory.CreateClient();

        protected async Task<HttpResponseMessage> Post(string url, string args = "")
        {
            var client = GetClient();
            var content = new StringContent("");
            content.Headers.Add("mc.args", args);
            return await client.PostAsync(url, content);
        }
    }
}