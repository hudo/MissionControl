using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MissionControl.Host.AspnetCore.Tests.Integration.Infrastructure
{
    public class Startup 
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMissionControl();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMissingControl(x =>
            {
                x.Url = "/mc";
                x.Authentication = req => true;
            });
        }
    }
}