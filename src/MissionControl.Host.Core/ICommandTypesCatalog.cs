using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MissionControl.Host.Core
{
    public interface ICommandTypesCatalog
    {
        void ScanAssemblies(Assembly[] assemblies, IServiceCollection services);

        CommandRegistration[] RegisteredCommands { get; }
    }
}