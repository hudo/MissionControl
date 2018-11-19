using System;
using System.Collections.Async;
using System.Threading.Tasks;

namespace MissionControl.Host.Core.Responses
{
    public class AsyncMultipartResponse : CliResponse
    {
        private readonly Func<AsyncEnumerator<string>.Yield, Task> _responses;

        public AsyncMultipartResponse(Func<AsyncEnumerator<string>.Yield, Task> responses)
        {
            _responses = responses;
        }

        public IAsyncEnumerable<string> Responses()
        {
            return new AsyncEnumerable<string>(_responses);
            
        }
        
        public override string Type => "multipart";
    }
}