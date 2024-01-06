using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace RS.MF.Aggregator.Core.Extensions
{
    public class HttpCall
    {
        public HttpCall(string uri, string verb, string payload, string successIf)
        {
            Uri = ForceToHttp(uri);
            Verb = new HttpMethod(verb.ToUpper());
            Payload = payload;
            SuccessIf = JsonConvert.DeserializeObject<Dictionary<string, string>>(successIf);
        }

        public Uri Uri { get; private set; }
        public HttpMethod Verb { get; private set; }
        public string Payload { get; private set; }
        public Dictionary<string, string> SuccessIf { get; private set; }

        private static Uri ForceToHttp(string requestUrl)
        {
            var uri = new UriBuilder(requestUrl);

            var hadDefaultPort = uri.Uri.IsDefaultPort;

            uri.Scheme = Uri.UriSchemeHttp;

            uri.Port = hadDefaultPort ? -1 : uri.Port;

            return uri.Uri;
        }

        public IEnumerable<string> Succeeded(string responseJsonString, HttpStatusCode httpStatusCode)
        {
            var isSuccessStatusCode = httpStatusCode == HttpStatusCode.OK || httpStatusCode == HttpStatusCode.Accepted;

            if (isSuccessStatusCode == false)
            {
                yield return string.Format("Call to the end point {0} retruned with http status code {1}", Uri, httpStatusCode);
                yield break;
            }

            if (string.IsNullOrWhiteSpace(responseJsonString))
            {
                yield break;
            }

            IEnumerable<string> failures;

            if (responseJsonString.Trim().StartsWith("["))
            {
                failures = VerifyConditionsWithJsonArrayResponse(responseJsonString);
            }
            else
            {
                failures = VerifyConditionsWithJsonObjectResponse(responseJsonString);
            }

            foreach (var failure in failures)
            {
                yield return failure;
            }
        }

        private IEnumerable<string> VerifyConditionsWithJsonObjectResponse(string responseJsonString)
        {
            bool jsonResponseInvalid = false;

            var responseJsonObject = new JObject();

            try
            {
                responseJsonObject = JObject.Parse(responseJsonString);
            }
            catch
            {
                jsonResponseInvalid = true;
            }

            if (jsonResponseInvalid)
            {
                yield return string.Format("Unable to parse response json string {0}", responseJsonString);
                yield break;
            }

            foreach (var condition in SuccessIf)
            {
                var responseValue = !condition.Key.Contains(".") ? responseJsonObject[condition.Key] : responseJsonObject.SelectToken(condition.Key);

                var conditionMeet = responseValue != null ? condition.Value.Equals(responseValue.ToString()) : false;

                if (!conditionMeet)
                {
                    yield return string.Format("Expected value {0} for {1}  did not match for calling {2}. Retruned {3}", condition.Value, condition.Key, Uri, responseValue ?? "null");
                }
            }
        }

        private IEnumerable<string> VerifyConditionsWithJsonArrayResponse(string responseJsonString)
        {
            bool jsonResponseInvalid = false;

            var responseJsonObject = new JArray();

            try
            {
                responseJsonObject = JArray.Parse(responseJsonString);
            }
            catch
            {
                jsonResponseInvalid = true;
            }

            if (jsonResponseInvalid)
            {
                yield return string.Format("Unable to parse response json string {0}", responseJsonString);
                yield break;
            }

            foreach (var condition in SuccessIf)
            {
                var responseValue = (string)responseJsonObject[condition.Key];

                var conditionMeet = condition.Value.Equals(responseValue);

                if (!conditionMeet)
                {
                    yield return string.Format("Expected value {0} for {1}  did not match for calling {2}. Retruned {3}", condition.Value, condition.Key, Uri, responseValue);
                }
            }
        }
    }
}