using System.Collections.Generic;
using System.Threading.Tasks;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core.Contracts.Pipeline
{
    public class PipelinePreBehavior<T> : IPipelineBehavior<T> where T:CliCommand
    {
        private readonly IEnumerable<IPipelinePreBehavior<T>> _preBehaviors;

        public PipelinePreBehavior(IEnumerable<IPipelinePreBehavior<T>> preBehaviors)
        {
            _preBehaviors = preBehaviors;
        }

        public async Task<CliResponse> Process(T command, CliHandlerDelegate next)
        {
            foreach (var preBehavior in _preBehaviors)
            {
                await preBehavior.Process(command);
            }

            return await next();
        }
    }
}