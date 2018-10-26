using System.Reflection;

namespace MissionControl.Host.Core
{
    public interface ICommandTypesCatalog
    {
        void DiscoverCommands(Assembly[] assemblies);

        CommandRegistration[] RegisteredCommands { get; }
    }
}