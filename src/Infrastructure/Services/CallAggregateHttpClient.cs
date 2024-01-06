using Microsoft.Extensions.Logging;
using RedlimeSolutions.Microservice.Framework;
using RS.MF.Aggregator.Application.ServicesContracts;
using RS.MF.Aggregator.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RS.MF.Aggregator.Infrastructure.Services
{
    public class CallAggregateHttpClient : ICallAggregateHttpClient
    {
        private readonly IServiceClient serviceClient;
        private readonly ILogger<CallAggregateHttpClient> logger;

        private static readonly IEnumerable<string> CallError = new string[] { "Call error" };
        private static readonly string[] CallCancelled = new string[] { "Call cancelled" };

        public CallAggregateHttpClient(IServiceClient serviceClient, ILogger<CallAggregateHttpClient> logger)
        {
            this.logger = logger;
            this.serviceClient = serviceClient;
        }

        public async Task<HttpAggregateCallResponse> CallAggregateAsync(IEnumerable<HttpCall> calls, string oauthBearerToken,CancellationToken cancellationToken)
        {
            logger.LogInformation($"Access token being used: {oauthBearerToken}");

            int callIndex = 0;

            bool hasStageFailure = false;

            var callAggregateCommandResponse = new HttpAggregateCallResponse();

            var stopWatch = new Stopwatch();

            foreach (var call in calls)
            {
                if (hasStageFailure)
                {
                    callAggregateCommandResponse.AddHttpCallResult(call.Uri.ToString(), CallCancelled, string.Empty, call.Payload, callIndex++, TimeSpan.FromTicks(0), call.Verb.Method);
                    continue;
                }

                stopWatch.Reset();

                stopWatch.Start();

                using (var request = PrepareHttpCallRequest(call, oauthBearerToken))
                {
                    using (var response = await serviceClient.SendToHttpAsync(request))
                    {
                        var responseJsonString = await response.Content.ReadAsStringAsync();

                        var failures = call.Succeeded(responseJsonString, response.StatusCode).ToArray();

                        stopWatch.Stop();

                        var hasFailure = failures.Any();

                        callAggregateCommandResponse.AddHttpCallResult(call.Uri.ToString(), hasFailure ? CallError : Enumerable.Empty<string>(), responseJsonString, call.Payload, callIndex, stopWatch.Elapsed, call.Verb.Method);

                        if (hasFailure)
                        {
                            hasStageFailure = true;

                            callAggregateCommandResponse.SetHttpCallFailedIndex(callIndex);

                            LogFailures(call, failures);
                        }
                        callIndex++;
                    }
                }
            }

            return callAggregateCommandResponse;
        }

        private HttpRequestMessage PrepareHttpCallRequest(HttpCall call, string oauthBearerToken)
        {
            HttpRequestMessage request = new HttpRequestMessage(call.Verb, call.Uri);

            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", oauthBearerToken);

            if (call.Verb == HttpMethod.Post)
            {
                request.Content = new StringContent(call.Payload, Encoding.UTF8, "application/json");
            }
            return request;
        }

        private void LogFailures(HttpCall httpCall, IEnumerable<string> failures)
        {
            foreach (var failure in failures)
            {
                var errorMessage = $"Error {failure} in calling: [{httpCall.Verb}] {httpCall.Uri}";

                logger.LogError(errorMessage);
            }
        }
    }
}