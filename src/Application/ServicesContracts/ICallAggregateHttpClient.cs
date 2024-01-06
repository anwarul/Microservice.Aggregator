using RS.MF.Aggregator.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RS.MF.Aggregator.Application.ServicesContracts
{
    public interface ICallAggregateHttpClient
    {
        Task<HttpAggregateCallResponse> CallAggregateAsync(IEnumerable<HttpCall> calls, string oauthBearerToken, CancellationToken cancellationToken);
    }
}
