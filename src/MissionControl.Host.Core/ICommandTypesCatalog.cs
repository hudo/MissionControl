using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MissionControl.Host.Core.Contracts;

namespace MissionControl.Host.Core
{
    public interface ICommandTypesCatalog
    {
        void ScanAssemblies(Assembly[] assemblies, IServiceCollection services);

        (Type type, CliCommandAttribute attributes) GetTypeByCommandName(string name);
    }
}