using RS.MF.Aggregator.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS.MF.Aggregator.Application.ServicesContracts
{
    public interface IHttpCallCommandValidator
    {
        IEnumerable<string> Validate(IEnumerable<HttpCallRequest> httpCalls);
    }
}
