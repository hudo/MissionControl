using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MissionControl.Host.AspnetCore.Tests.Integration.Infrastructure;
using MissionControl.Host.Core.Contracts;
using Newtonsoft.Json;
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

        protected async Task<(HttpResponseMessage HttpResponse, R Item)> Post<R>(string url, string args = "") where R:class
        {
            var response = await Post(url, args);
            var body = await response.Content.ReadAsStringAsync();
            var item = JsonConvert.DeserializeObject<R>(body);
            return (response, item);
        }
    }
}