using Microsoft.Extensions.DependencyInjection;
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
            _catalog.DiscoverCommands(new[] {this.GetType().Assembly});
            
            var command = _catalog.FindCommandByName("foo");
            
            command.IsSome.ShouldBeTrue();
            command.Value.Type.ShouldBe(typeof(FooCommand));
        }

        [Fact]
        public void Unknown_command_name_returns_null()
        {
            _catalog.DiscoverCommands(new []{ GetType().Assembly});

            var command = _catalog.FindCommandByName("foobar");

            command.IsNull.ShouldBeTrue();
        }

        [CliCommand("foo")]
        private class FooCommand : CliCommand {  }
        
        [CliCommand("bar")]
        private class BarCommand : CliCommand {  }
    }
}