using RS.MF.Aggregator.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RS.MF.Aggregator.Core.Extensions
{
    public class HttpAggregateCallResponse
    {
        private string executionError;
        public int FailedCallIndex { get; private set; }

        private List<HttpCallResult> httpAggregateCallResults;

        public bool AggregateCallCompleted
        {
            get
            {
                return FailedCallIndex < 0
                    && (CallValidationErrors == null || CallValidationErrors.Any() == false)
                    && string.IsNullOrWhiteSpace(executionError);
            }
        }

        public IEnumerable<HttpCallResult> HttpAggregateCallResults
        { get { return httpAggregateCallResults; } }

        public GenericEventPublicationResponse GenericEventPublicationResponse { get; private set; }

        public HttpAggregateCallResponse()
        {
            FailedCallIndex = -1;
            httpAggregateCallResults = new List<HttpCallResult>();
        }

        public void AddHttpCallResult(string endPoint, IEnumerable<string> failures, string actualServiceResponse, string payload, int callIndex, TimeSpan executionTime, string verb)
        {
            var httpCallResult = new HttpCallResult(endPoint, failures, actualServiceResponse, payload, callIndex, executionTime, verb);

            httpAggregateCallResults.Add(httpCallResult);
        }

        public void SetHttpCallFailedIndex(int failedCallIndex)
        {
            FailedCallIndex = failedCallIndex;
        }

        public string ExecutionError
        { get { return executionError; } }

        public void SetExecutionError(string executionError)
        {
            this.executionError = executionError;
        }

        public IEnumerable<string> CallValidationErrors { get; private set; }

        public void SetValidationErrors(IEnumerable<string> callValidationErrors)
        {
            CallValidationErrors = callValidationErrors;
        }

        public void SetGenericEventPublicationResponse(GenericEventPublicationResponse genericEventPublicationResponse)
        {
            GenericEventPublicationResponse = genericEventPublicationResponse;
        }
    }
}