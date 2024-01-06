using System;
using System.Collections.Generic;
using System.Linq;

namespace RS.MF.Aggregator.Core.Entities
{
    public class HttpCallResult
    {
        public HttpCallResult(string endPoint, IEnumerable<string> failures, string serviceResponse, string payload, int callIndex, TimeSpan executionTime, string verb)
        {
            EndPoint = endPoint;
            Failures = failures;
            Response = serviceResponse;
            Payload = payload;
            CallIndex = callIndex;
            Success = !failures.Any();
            ExecutionTime = executionTime;
            Verb = verb;
        }

        public string Verb { get; private set; }
        public string EndPoint { get; private set; }
        public IEnumerable<string> Failures { get; private set; }
        public string Response { get; private set; }
        public string Payload { get; private set; }
        public int CallIndex { get; private set; }
        public bool Success { get; private set; }
        public TimeSpan ExecutionTime { get; private set; }
    }
}