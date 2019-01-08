using System.Linq;
using System.Threading.Tasks;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core.StandardCommands
{
    [CliCommand("list-command", help: "List of registered commands")]
    internal class ListCommandsCommand : CliCommand
    {

    }

    internal class ListCommandsHandler : ICliCommandHandler<ListCommandsCommand>
    {
        private readonly ICommandTypesCatalog _catalog;

        public ListCommandsHandler(ICommandTypesCatalog catalog)
        {
            _catalog = catalog;
        }
        
        public Task<CliResponse> Handle(ListCommandsCommand command)
        {
            var response = $"Registered commands:\n{string.Join("\n", _catalog.RegisteredCommands.Where(x => x.Attribute.Visible).Select(x => x.Name))}"; 
            return Task.FromResult<CliResponse>(new TextResponse(response));
        }
    }
}