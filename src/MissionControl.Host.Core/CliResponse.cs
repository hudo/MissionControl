using System;

namespace MissionControl.Host.Core
{
    public abstract class CliResponse
    {
        protected CliResponse()
        {
            Id = Guid.NewGuid();    
        }
        
        public Guid Id { get; }
    }
    
    public sealed class TextResponse : CliResponse  {  } // text + CRLF
    public sealed class UpdateLineResponse : CliResponse { } // X pos + test
    
    public sealed class SagaResponse : CliResponse
    {
        public readonly IObservable<CliResponse> Observable;

        public SagaResponse(IObservable<CliResponse> observable)
        {
            Observable = observable;
        }
    }
}