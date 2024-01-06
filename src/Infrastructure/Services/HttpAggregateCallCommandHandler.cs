using RedlimeSolutions.Microservice.Framework;
using RS.MF.Aggregator.Application.Commands;
using RS.MF.Aggregator.Application.Dtos;
using RS.MF.Aggregator.Application.ServicesContracts;
using RS.MF.Aggregator.Core.Contracts;
using RS.MF.Aggregator.Core.Entities;
using RS.MF.Aggregator.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RS.MF.Aggregator.Infrastructure.Services
{
    public class HttpAggregateCallCommandHandler : IHttpAggregateCallCommandHandler
    {
        private readonly IHttpCallCommandValidator httpCallValidator;
        private readonly ISecurityContextProvider securityContextProvider;
        private readonly ICallAggregateHttpClient callAggregateHttpClient;
        private readonly IOauthBearerTokenProvider oauthBearerTokenProvider;
        private readonly IGenericEventDataValidator genericEventDataValidator;

        public HttpAggregateCallCommandHandler(IHttpCallCommandValidator httpCallValidator, ISecurityContextProvider securityContextProvider, ICallAggregateHttpClient callAggregateHttpClient, IOauthBearerTokenProvider oauthBearerTokenProvider, IGenericEventDataValidator genericEventDataValidator)
        {
            this.httpCallValidator = httpCallValidator;
            this.securityContextProvider = securityContextProvider;
            this.callAggregateHttpClient = callAggregateHttpClient;
            this.oauthBearerTokenProvider = oauthBearerTokenProvider;
            this.genericEventDataValidator = genericEventDataValidator;
        }

        public async Task<HttpAggregateCallResponse> HandleAsync(HttpAggregateCallCommand command, CancellationToken cancellationToken)
        {
            var callAggregateResponse = new HttpAggregateCallResponse();

            if (command == null)
            {
                callAggregateResponse.SetValidationErrors(new string[] { "Command is null" });
                return callAggregateResponse;
            }

            var validationErrors = httpCallValidator.Validate(command.HttpCalls);

            if (validationErrors.Any())
            {
                callAggregateResponse.SetValidationErrors(validationErrors);
                return callAggregateResponse;
            }

            validationErrors = genericEventDataValidator.Validate(command.GenericEventData);

            if (validationErrors.Any())
            {
                callAggregateResponse.SetValidationErrors(validationErrors);
                return callAggregateResponse;
            }

            var httpCalls = HttpCallMapper.Map(command.HttpCalls);

            try
            {
                var accessToken = securityContextProvider.GetSecurityContext().OauthBearerToken;//await oauthBearerTokenProvider.GetLongLivingTokenTokenForCurrent();

                callAggregateResponse = await callAggregateHttpClient.CallAggregateAsync(httpCalls, accessToken,cancellationToken);

                if (command.GenericEventData == null)
                {
                    callAggregateResponse.SetGenericEventPublicationResponse(new GenericEventPublicationResponse
                    {
                        EventType = "Not Specified",
                    });
                }
                else
                {
                    if (callAggregateResponse.AggregateCallCompleted)
                    {
                        /*var genericEventPublicationResponse = await PublishGenericEventAsync(command.GenericEventData);

                        callAggregateResponse.SetGenericEventPublicationResponse(genericEventPublicationResponse);*/
                    }
                    else
                    {
                        callAggregateResponse.SetGenericEventPublicationResponse(new GenericEventPublicationResponse
                        {
                            Published = false,
                            Errors = new string[] { "Not published since there is aggregate call error" },
                            EventType = command.GenericEventData.EventType
                        });
                    }
                }
            }
            catch (AggregateException e)
            {
                callAggregateResponse.SetExecutionError(e.InnerException.Message);
            }

            return callAggregateResponse;
        }
    }
}
