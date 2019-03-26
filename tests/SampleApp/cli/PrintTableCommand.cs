using System.Threading.Tasks;
using MissionControl.Host.Core.Contracts;
using MissionControl.Host.Core.Responses;
using System.Collections.Generic;

namespace SampleApp.cli
{
    [CliCommand("print-table", "Print sample table")]
    public class PrintTableCommand : CliCommand
    {
         
    }

    public class PrintTableCommandHandler : ICliCommandHandler<PrintTableCommand>
    {
        
        public Task<CliResponse> Handle(PrintTableCommand command)
        {
            var model = new TableResponse<RowModel>() 
            {
                Rows = new List<RowModel> 
                {
                    new RowModel() { Id = 1, Title = "first", Amount = 10 },
                    new RowModel() { Id = 2, Title = "second", Amount = 15.4m },
                    new RowModel() { Id = 3, Title = "third", Amount = 0.01m }
                }
            };

            return Task.FromResult((CliResponse)model);
        }

        public class RowModel 
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public decimal Amount { get; set; }
        }
    }
}