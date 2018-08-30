using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MissionControl.Host.Core.Contracts;
using Moq;
using Shouldly;
using Xunit;

namespace MissionControl.Host.Core.Tests.Unit
{
    public class CommandTypeCatalogTests
    {
        private readonly Mock<IServiceCollection> _servicesMock = new Mock<IServiceCollection>();
        private readonly CommandTypesCatalog _catalog;

        public CommandTypeCatalogTests()
        {
            _catalog = new CommandTypesCatalog();
        }

        [Fact]
        public void Scan_and_find_known_command()
        {
            _catalog.ScanAssemblies(new[] {this.GetType().Assembly}, _servicesMock.Object);
            
            var (type, _) = _catalog.GetTypeByCommandName("foo");
            
            type.ShouldBe(typeof(FooCommand));
        }

        [Fact]
        public void Unknown_command_name_returns_null()
        {
            _catalog.ScanAssemblies(new []{ GetType().Assembly}, _servicesMock.Object);

            var (type, _) = _catalog.GetTypeByCommandName("foobar");

            type.ShouldBeNull();
        }

        [CliCommand("foo")]
        private class FooCommand : CliCommand {  }
        
        [CliCommand("bar")]
        private class BarCommand : CliCommand {  }
    }
}