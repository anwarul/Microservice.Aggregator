using System;
using System.Collections.Generic;
using System.Text;

namespace RS.MF.Aggregator.Core.Entities
{
    public class GenericEventPublicationResponse
    {
        public int StatusCode
        {
            get
            {
                return Errors != null && Errors.Length > 0 ? 1 : 0;
            }
        }
        public bool Published { get; set; }
        public string EventType { get; set; }
        public string[] Errors { get; set; }
    }

}
