using Newtonsoft.Json.Linq;
using RedlimeSolutions.Microservice.Framework;
using RS.MF.Aggregator.Application.ServicesContracts;
using RS.MF.Aggregator.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS.MF.Aggregator.Infrastructure.Validation
{
    public class HttpCallCommandValidator: IHttpCallCommandValidator
    {
        private readonly string[] restrictedMicroServiceHosts;

        private readonly IWhiteListedHostRepository whiteListedHostRepository;

        public HttpCallCommandValidator(IAppSettings appSettings, IWhiteListedHostRepository whiteListedHostRepository)
        {
            restrictedMicroServiceHosts = new string[]
            {
                //new Uri(appSettings.StsEndPointUri).Host,
                //new Uri(appSettings.ShortMessageServiceBaseUrl).Host,
            };

            this.whiteListedHostRepository = whiteListedHostRepository;
        }

        public IEnumerable<string> Validate(IEnumerable<HttpCallRequest> httpCallCommands)
        {
            var thereIsNoHttpCalls = httpCallCommands.Any() == false;

            if (thereIsNoHttpCalls)
            {
                yield return "There are no http calls to execute";
            }

            if (httpCallCommands.Count() > 100)
            {
                yield return "More than 100 http calls are not allowed";
            }

            foreach (var httpCallCommand in httpCallCommands)
            {
                if (string.IsNullOrWhiteSpace(httpCallCommand.Uri))
                {
                    yield return "Uri cannot be empty";
                }

                if (string.IsNullOrWhiteSpace(httpCallCommand.Uri) == false && ValidateUri(httpCallCommand.Uri) == false)
                {
                    yield return string.Format("Uri {0} is invalid", httpCallCommand.Uri);
                }

                if (IsRestrictedDomainUri(httpCallCommand.Uri) == true)
                {
                    yield return string.Format("Call to Uri {0} is restricted through Service-Aggregator", httpCallCommand.Uri);
                }

                if (IsAWhiteListedHost(httpCallCommand.Uri) == false)
                {
                    yield return string.Format($"Host of uri {httpCallCommand.Uri} is not white-listed therefore call is restricted through Service-Aggregator", httpCallCommand.Uri);
                }


                if (string.IsNullOrWhiteSpace(httpCallCommand.Verb))
                {
                    yield return string.Format("Verb cannot be empty. {0}", httpCallCommand.Uri);
                }
                if (string.IsNullOrWhiteSpace(httpCallCommand.Verb) == false && httpCallCommand.Verb.Equals("get", StringComparison.InvariantCultureIgnoreCase) == false && httpCallCommand.Verb.Equals("post", StringComparison.InvariantCultureIgnoreCase) == false)
                {
                    yield return string.Format("Invalid verb {0} for {1}. Only Get and Post are supported", httpCallCommand.Verb, httpCallCommand.Uri);
                }

                /*if (string.IsNullOrWhiteSpace(httpCallCommand.Payload))
                {
                    yield return "Payload cannot be empty";
                }

                if (string.IsNullOrWhiteSpace(httpCallCommand.Payload) == false && IsValidJson(httpCallCommand.Payload) == false)
                {
                    yield return "Payload Invalid";
                }*/

                if (string.IsNullOrWhiteSpace(httpCallCommand.SuccessIf))
                {
                    yield return "SuccessIf cannot be empty";
                }

                if (string.IsNullOrWhiteSpace(httpCallCommand.SuccessIf) == false && IsValidJson(httpCallCommand.SuccessIf) == false)
                {
                    yield return "SuccessIf Invalid";
                }
            }

            yield break;
        }

        private bool IsRestrictedDomainUri(string uri)
        {
            var uriCreated = Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out Uri createdUri);

            if (uriCreated == false)
            {
                return uriCreated;
            }

            return restrictedMicroServiceHosts.Any(rmsh => rmsh.Equals(createdUri.Host, StringComparison.InvariantCultureIgnoreCase));
        }

        private bool IsAWhiteListedHost(string uri)
        {
            var uriCreated = Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out Uri createdUri);

            if (uriCreated == false)
            {
                return uriCreated;
            }

            return whiteListedHostRepository.IsWhiteListedHost(createdUri.Host);
        }



        private static bool ValidateUri(string uri)
        {
            return Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out Uri createdUri);
        }
        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch
                {

                }
            }
            return false;
        }

    }
}
