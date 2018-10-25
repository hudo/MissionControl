using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Utilities;

namespace MissionControl.Host.Core
{
    internal interface IRequestParser
    {
        Maybe<CliCommand> Parse(Request request);
    }
}