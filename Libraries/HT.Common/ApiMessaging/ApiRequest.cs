using System.Collections.Generic;
using Newtonsoft.Json;

namespace HT.Common.ApiMessaging
{
    /// <summary>
    /// Provides common fields for API requests.  Inherit from this class to build custom models
    /// </summary>
    public class ApiRequest
    {
        [JsonProperty(PropertyName="meta", NullValueHandling = NullValueHandling.Ignore)]
        public SortedDictionary<string,string> Meta { get; set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }

        public ApiRequest()
        {
              
        }
        public bool ShouldSerializeMeta()
        {
            return Meta != null && Meta.Count > 0;
        }
    }

    public class ApiRequest<TBody>:ApiRequest
    {
        public ApiRequest():base()
        {

        }
        [JsonProperty("data",NullValueHandling = NullValueHandling.Ignore)]
        public new TBody Data
        {
            get
            {
                return (TBody) base.Data;
            }
            set
            {
                base.Data = value;
            }
        }
    }
}
