using System;

namespace MissionControl.Host.Core
{

    public class ClientResponse
    {
        public CliResponse CommandResponse { get; set; }
        public string Host { get; set; }
        public int ErrorCode { get; set; }
        public bool HasError => ErrorCode > 0;
        public string ErrorMessage { get; set; }
    }
    
    public abstract class CliResponse
    {
        protected CliResponse()
        {
            Id = Guid.NewGuid();    
        }
        
        public Guid Id { get; }
        public Guid RequestId { get; set; }
    }

    public sealed class TextResponse : CliResponse
    {
        public TextResponse(string content)
        {
            Content = content;
        }
        
        public string Content { get; set; }
    } 

    public sealed class UpdateLineResponse : CliResponse
    {
        public int CharPosition { get; set; }

        public string Content { get; set; }
    } 
    
    
    public sealed class SagaResponse : CliResponse // todo
    {
        public readonly IObservable<CliResponse> Observable;

        public SagaResponse(IObservable<CliResponse> observable)
        {
            Observable = observable;
        }
    }
    
}