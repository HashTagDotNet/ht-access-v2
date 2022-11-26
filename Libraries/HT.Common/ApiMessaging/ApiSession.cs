using System.Security.Claims;
using Newtonsoft.Json;

namespace HT.Common.ApiMessaging
{
    /// <summary>
    /// Represents a single request / response pair on an API server.  Sometimes used for logging but rarely returned to caller of API
    /// </summary>
    public class ApiSession
    {
        public string SessionID { get; set; }
        public ApiResponse Response { get; set; }
        public ApiRequest Request { get; set; }
        
        private ApiEvents _events;

        /// <summary>
        /// List of messages (called 'events') associated with this session.  Helpful for collecting 
        /// events such as exceptions during session for logging upon exit of call
        /// </summary>
        [JsonProperty("events", NullValueHandling = NullValueHandling.Ignore)]
        public ApiEvents Events
        {
            get
            {
                if (_events != null)
                {
                    return _events;
                }
                _events = new ApiEvents();
                return _events;
            }
            set
            {
                _events = value;
            }
        }

        /// <summary>
        /// The authenticated user sending this request.  Often set using an application wide filter.  Very handy when having to track who performed what action.  NOTE:  For security purposes
        /// ignored on Json serialization
        /// </summary>
        [JsonIgnore]
        public ClaimsPrincipal Actor { get; set; }
    }

    public class ApiSession<TRequestBody, TResponseBody>:ApiSession
    {
        public new ApiResponse<TResponseBody> Response { get; set; }
        public new ApiRequest<TRequestBody> Request { get; set; }
    }
}
