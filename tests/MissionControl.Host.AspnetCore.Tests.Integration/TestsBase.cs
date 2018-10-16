using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using MissionControl.Host.AspnetCore.Tests.Integration.Infrastructure;
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
    }
}