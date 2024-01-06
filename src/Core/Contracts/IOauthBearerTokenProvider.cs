using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RS.MF.Aggregator.Core.Contracts
{
    public interface IOauthBearerTokenProvider
    {
        Task<string> GetLongLivingTokenTokenForCurrent();
    }
}
