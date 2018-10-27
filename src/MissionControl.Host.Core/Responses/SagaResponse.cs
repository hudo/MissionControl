using System;

namespace MissionControl.Host.Core.Responses
{
    public sealed class SagaResponse : CliResponse // todo: design saga 
    {
        public readonly IObservable<CliResponse> Observable;

        public SagaResponse(IObservable<CliResponse> observable)
        {
            Observable = observable;
        }

        public override string Type => "saga";
    }
}