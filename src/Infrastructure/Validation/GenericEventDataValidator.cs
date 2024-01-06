using Newtonsoft.Json.Linq;
using RS.MF.Aggregator.Application.Dtos;
using RS.MF.Aggregator.Application.ServicesContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS.MF.Aggregator.Infrastructure.Validation
{
    public class GenericEventDataValidator: IGenericEventDataValidator
    {
        public IEnumerable<string> Validate(GenericEventData genericEventData)
        {
            if (genericEventData == null)
            {
                yield break;
            }

            if (string.IsNullOrWhiteSpace(genericEventData.EventType))
            {
                yield return "Event type cannot be null or empty for generic event data.";
            }

            if (string.IsNullOrWhiteSpace(genericEventData.EventPayload))
            {
                yield return "Event payload is require.";
            }

            if (IsValidJson(genericEventData.EventPayload) == false)
            {
                yield return "Event payload is invalid.";
            }

            yield break;
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
