using System;
using System.Collections.Generic;
using System.Text;

namespace RS.MF.Aggregator.Core.Entities
{
    public class HttpCallRequest
    {
        /// <summary>
        /// command. Uri: Uniform Resource Identifier of the aggregated call.
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// command. Verb: The request method for executing call. For example - http Post, http Get etc.
        /// </summary>
        public string Verb { get; set; }
        /// <summary>
        /// command. Payload: The payload content for executing call.
        /// </summary>
        public string Payload { get; set; }
        /// <summary>
        /// command. SuccessIf: The criteria by which payload is accepted. If one entry value is deemed incorrect, the remaining entries (in sequence) will likely fail.
        /// </summary>
        public string SuccessIf { get; set; }
    }

}
