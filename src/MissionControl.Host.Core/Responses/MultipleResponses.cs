using System;
using System.Collections.Async;
using System.Threading.Tasks;

namespace MissionControl.Host.Core.Responses
{
    public class MultipleResponses : CliResponse // todo: rename
    {
        private readonly Func<AsyncEnumerator<string>.Yield, Task> _responses;

        public MultipleResponses(Func<AsyncEnumerator<string>.Yield, Task> responses)
        {
            _responses = responses;
            IsDone = false;
        }

        public IAsyncEnumerable<string> Responses()
        {
            return new AsyncEnumerable<string>(_responses);
            
        }
        
        public override string Type => "multipart";
    }
}