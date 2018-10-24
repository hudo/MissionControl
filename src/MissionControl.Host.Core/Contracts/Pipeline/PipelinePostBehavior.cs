using System.Collections.Generic;
using System.Threading.Tasks;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core.Contracts.Pipeline
{
    public class PipelinePostBehavior<T> : IPipelineBehavior<T> where T:CliCommand
    {
        private readonly IEnumerable<IPipelinePostBehavior<T>> _postBehaviors;

        public PipelinePostBehavior(IEnumerable<IPipelinePostBehavior<T>> postBehaviors)
        {
            _postBehaviors = postBehaviors;
        }
        
        public async Task<CliResponse> Process(T command, CliHandlerDelegate next)
        {
            var response = await next();

            foreach (var postBehavior in _postBehaviors)
            {
                await postBehavior.Process(command, response);
            }

            return response;
        }
    }
}