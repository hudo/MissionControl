using System;
using System.Linq;
using System.Threading.Tasks;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core.Contracts.StandardCommands
{
    internal class ListCommandsHandler : ICliCommandHandler<ListCommandsCommand>
    {
        private readonly ICommandTypesCatalog _catalog;

        public ListCommandsHandler(ICommandTypesCatalog catalog)
        {
            _catalog = catalog;
        }
        
        public Task<CliResponse> Handle(ListCommandsCommand command)
        {
            var br = Environment.NewLine;
            var response = $"Registered commands:{br}{string.Join(br, _catalog.RegisteredCommands.Select(x => x.Name))}"; 
            return Task.FromResult<CliResponse>(new TextResponse(response));
        }
    }
}