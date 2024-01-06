using RS.MF.Aggregator.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS.MF.Aggregator.Core.Extensions
{
    public static class HttpCallMapper
    {
        public static IEnumerable<HttpCall> Map(IEnumerable<HttpCallRequest> httpCallCommands)
        {
            foreach (var httpCallCommand in httpCallCommands)
            {
                yield return new HttpCall(httpCallCommand.Uri, httpCallCommand.Verb, httpCallCommand.Payload, httpCallCommand.SuccessIf);
            }
        }
    }

}
