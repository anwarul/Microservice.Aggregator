using RS.MF.Aggregator.Application.Dtos;
using RS.MF.Aggregator.Core.Entities;

namespace RS.MF.Aggregator.Application.Commands
{
    public class HttpAggregateCallCommand
    {
        public HttpAggregateCallCommand()
        {
            HttpCalls = new HttpCallRequest[] { };
        }

        public HttpCallRequest[] HttpCalls { get; set; }

        public GenericEventData GenericEventData { get; set; }
    }
}