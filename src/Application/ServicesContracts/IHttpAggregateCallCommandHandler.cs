using RS.MF.Aggregator.Application.Commands;
using RS.MF.Aggregator.Core.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace RS.MF.Aggregator.Application.ServicesContracts
{
    public interface IHttpAggregateCallCommandHandler
    {
        Task<HttpAggregateCallResponse> HandleAsync(HttpAggregateCallCommand command, CancellationToken cancellationToken);
    }
}