using System.Diagnostics;
using System.Threading.Tasks;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core.StandardCommands
{
    [CliCommand("server-info", "Display server information")]
    internal class ServerInfoCommand : CliCommand
    {

    }

    internal class ServerInfoCommandHandler : ICliCommandHandler<ServerInfoCommand>
    {
        public Task<CliResponse> Handle(ServerInfoCommand command)
        {
            var newLine = "\n";
            var proc = Process.GetCurrentProcess();

            var content = $"Working set: {proc.WorkingSet64 / 1024:N1} kb {newLine}"
                          + $"Peak virtual memory size: {proc.PeakWorkingSet64 / 1024:N1} kb {newLine}"
                          + $"Private memory size: {proc.PrivateMemorySize64 / 1024:N1} kb {newLine}"
                          + $"Virtual memory size: {proc.VirtualMemorySize64 / 1024:N1} kb {newLine}"
                          + $"Total CPU time: {proc.TotalProcessorTime.TotalMinutes:F4} mins {newLine}"
                          + $"Machine name: {proc.MachineName}";

            return Task.FromResult<CliResponse>(new TextResponse(content));
        }
    }
}
