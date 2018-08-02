using System;
using System.Threading.Tasks;

namespace MissionControl.Host.Core
{
    public abstract class CliCommand
    {
    }

    public abstract class CliResponse 
    {
        protected CliResponse()
        {
            
        }   
    }

    public sealed class TextResponse : CliResponse  {  } // text + CRLF
    public sealed class UpdateLineResponse : CliResponse { } // X pos + test
    public sealed class SagaResponse : CliResponse { } // todo 
    
    public interface ICliCommandHandler<T> where T : CliCommand
    {
        // https://www.interact-sw.co.uk/iangblog/2013/11/29/async-yield-return
        
        IObservable<CliResponse> Handle<T>(T command);
    }
    
}
