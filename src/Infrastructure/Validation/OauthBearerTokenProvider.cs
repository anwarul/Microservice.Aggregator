using Microsoft.Extensions.Logging;
using RedlimeSolutions.Microservice.Framework;
using RS.MF.Aggregator.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RS.MF.Aggregator.Infrastructure.Validation
{
    public class OauthBearerTokenProvider : IOauthBearerTokenProvider
    {
        private readonly ILogger<OauthBearerTokenProvider> logger;

        private readonly ISecurityContextProvider securityContextProvider;

        //private readonly AccessTokenProvider oauthAccessTokenGenerator;

        public OauthBearerTokenProvider(
            ILogger<OauthBearerTokenProvider> logger,
            ISecurityContextProvider securityContextProvider/*,
            AccessTokenProvider oauthAccessTokenGenerator*/
            )
        {
            this.logger = logger;
            this.securityContextProvider = securityContextProvider;
            //this.oauthAccessTokenGenerator = oauthAccessTokenGenerator;
        }
        public async Task<string> GetLongLivingTokenTokenForCurrent()
        {
            var securityContext = securityContextProvider.GetSecurityContext();

            var currentRoles = string.Join(',', securityContext.Roles);

            logger.LogInformation($"Current roles: {currentRoles}");

            return "";//oauthAccessTokenGenerator.CreateFromAccessTokenAsync(securityContext.OauthBearerToken);
        }

    }
}
