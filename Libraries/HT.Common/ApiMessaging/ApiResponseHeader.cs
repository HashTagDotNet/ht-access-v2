using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace HT.Common.ApiMessaging
{
    /// <summary>
    /// Common fields which response(s) from an API SHOULD always return
    /// </summary>
    /// <example>
    /// {
    ///     "id":"24-sdfasf-12341-dasasf",
    ///     "timestamp": "2017-02-22T23:34:44.33-5",
    ///     "sessionId": "sdfa-2342-23424-79795b3",
    ///     "isOk": false,
    ///     "status": 200,
    ///     "statusCode": "OK",
    ///     "severity": 3,
    ///     "severityCode": "Error",
    ///     "maxEventIndex": 0,
    ///     "events": [
    ///      {
    ///         "id": "3433l213-2342-sfd",
    ///         "links":[
    ///             "details":"https://myerrorwebsite/errors/345-sdfas-2222"
    ///         ],
    ///         "status": 200,
    ///         "statusCode" : "OK",
    ///         "code": "APP-23-DSG-001",
    ///         "eventId": "30211",
    ///         "title":"Call service desk with reference #: '3433l213-2342-sfd'",
    ///         "detail":"...complete-call-stack, execution context, and machine parameters...",
    ///         "severity": 3,
    ///         "severityCode": "Error",
    ///         "isOk": false
    ///     }
    ///     ]
    /// }
    /// </example>
    public class ApiResponseHeader 
    {
        /// <summary>
        /// Unique identifier for this request/response pair.  Often used
        /// in logging scenarios.  NOTE:  All diagnostic log messages
        /// SHOULD include this ID to facilitate all messages
        /// generated for the request.  Logical local correlation identifier
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string ID { get; set; }

        /// <summary>
        /// Server local time which header was created
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Unique identifier for a caller's session.  Helpful for correlating
        /// log entries across multiple servers.  NOTE:  Diagnostic messages
        /// MAY include this ID in order facilitate rebuilding a request that
        /// crosses multiple servers or consumes different web services.
        /// </summary>
        [JsonProperty("sessionId", NullValueHandling = NullValueHandling.Ignore)]
        public string CorrelationID { get; set; }

        bool? isOk;
        /// <summary>
        /// True if response represents a non-error state.  Convenience property
        /// </summary>
        [JsonProperty("isOk", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsOk
        {
            get
            {
                if (isOk != null)
                {
                    return isOk.Value;
                }
                if (Events != null && Events.Count > 0)
                {
                    return Events.FirstOrDefault(e => !e.IsOk) == null;
                }
                return true;
            }
            set
            {
                isOk = value;
            }
        }

        /// <summary>
        /// Recommended HTTP status code the service wants to return to caller.  If
        /// not explicitly set (normal use-case), use default values based on maximum state of header messages. (e.g. 400, 200, 201)
        /// </summary>
        private HttpStatusCode? _status;
      //  [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        //public HttpStatusCode HttpStatus
        //{
        //    get
        //    {
        //        if (_status.HasValue)
        //        {
        //            return _status.Value;
        //        }
        //        var evtIndex = MaxEventIndex;
        //        if (!evtIndex.HasValue)
        //        {
        //            return HttpStatusCode.OK;
        //        }
        //        return Events[evtIndex.Value].HttpStatus;
        //    }
        //    set
        //    {
        //        _status = value;
        //    }
        //}

        /// <summary>
        /// String version of most severe HTTP Status (e.g. OK, FileNotFound)
        /// </summary>
        //[JsonProperty("statusCode", NullValueHandling = NullValueHandling.Ignore)]
        //[JsonConverter(typeof(StringEnumConverter))]
        //public HttpStatusCode HttpStatusCode
        //{
        //    get
        //    {
        //        return HttpStatus;
        //    }
        //    set
        //    {
        //        HttpStatus = value;
        //    }
        //}

        ApiEventStatus? _level;
        /// <summary>
        /// Numeric Level (e.g. Info=2, Warning=3, Error=4, Critical=5) service wishes to return to caller.
        /// Some callers (or APIs) might use this indicator instead of HTTP status as success indicator.
        /// If not specified, uses the maximum level of any included messages.
        /// </summary>
        //[JsonProperty("severity")]
        //public ApiEventStatus MessageStatus
        //{
        //    get
        //    {
        //        if (_level.HasValue)
        //        {
        //            return _level.Value;
        //        }
        //        var evtIndex = MaxEventIndex;

        //        if (!evtIndex.HasValue)
        //        {
        //            return ApiEventStatus.None;
        //        }
        //        return Events[evtIndex.Value].MessageStatus;
        //    }
        //    set
        //    {
        //        _level = value;
        //    }
        //}

    

        /// <summary>
        /// Text Level (e.g. Info, Warning, Error, Critical) service wishes to return to caller.
        /// </summary>
        //[JsonProperty("serverityCode")]
        //[JsonConverter(typeof(StringEnumConverter))]
        //public ApiEventStatus MessageStatusCode
        //{
        //    get
        //    {
        //        return MessageStatus;
        //    }
        //    set
        //    {
        //        MessageStatus = value;
        //    }
        //}

        /// <summary>
        /// return event index with most severe message (convenience method).  Often used
        /// in UI to display most severe message in some kind of notification
        /// </summary>
        //[JsonProperty("maxEventIndex", NullValueHandling = NullValueHandling.Ignore)]
        //public int? MaxEventIndex
        //{
        //    get
        //    {
        //        if (Events == null || Events.Count == 0)
        //        {
        //            return null;
        //        }
        //        return Events.MaxIndex(e => e.CombinedSeverity);
        //    }
        //    set
        //    {
        //        // place holder for serialization
        //    }
        //}

        private ApiEvents _events;
        /// <summary>
        /// List of messages (called 'events') associated with this call
        /// </summary>
        [JsonProperty("events", NullValueHandling = NullValueHandling.Ignore)]
        public ApiEvents Events {
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
        /// Well formatted, human readable, representation of this status
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);

        }

        //public bool ShouldSerializeLinks()
        //{
        //    switch (ApiSettings.Response.SerializeHeaderLinks)
        //    {
        //        case ApiSettings.Response.SerializeSwitch.Never: return false;
        //        case ApiSettings.Response.SerializeSwitch.NonEmpty: return _links != null && _links.Count > 0;
        //        case ApiSettings.Response.SerializeSwitch.SetOnly: return _links != null;
        //        case ApiSettings.Response.SerializeSwitch.Always: return true;
        //    }
        //    return true; //default to serialize

        //}
        //public bool ShouldSerializeMessages()
        //{
        //    switch (ApiSettings.Response.SerializeHeaderMessages)
        //    {
        //        case ApiSettings.Response.SerializeSwitch.Never: return false;
        //        case ApiSettings.Response.SerializeSwitch.NonEmpty: return _messages != null && _messages.Count > 0;
        //        case ApiSettings.Response.SerializeSwitch.SetOnly: return _links != null;
        //        case ApiSettings.Response.SerializeSwitch.Always: return true;
        //    }
        //    return true; //default to serialize
        //}

        //public bool ShouldSerializeActionStatus()
        //{
        //    switch(ApiSettings.Response.SerializeHeaderActionStatus)
        //    {
        //        case ApiSettings.Response.SerializeSwitch.Never: return false;
        //        case ApiSettings.Response.SerializeSwitch.NonEmpty: return true;
        //        case ApiSettings.Response.SerializeSwitch.SetOnly: return _actionStatus.HasValue;
        //        case ApiSettings.Response.SerializeSwitch.Always: return true;                
        //    }
        //    return true; //default to serialize
        //}
        //public bool ShouldSerializeApiStatus()
        //{
        //    switch (ApiSettings.Response.SerializeHeaderApiStatus)
        //    {
        //        case ApiSettings.Response.SerializeSwitch.Never: return false;
        //        case ApiSettings.Response.SerializeSwitch.NonEmpty: return true;
        //        case ApiSettings.Response.SerializeSwitch.SetOnly: return _actionStatus.HasValue;
        //        case ApiSettings.Response.SerializeSwitch.Always: return true;
        //    }
        //    return true; //default to serialize
        //}
        //public bool ShouldSerializeActionStatusCode()
        //{
        //    return ShouldSerializeActionStatus();
        //}
        //public bool ShouldSerializeApiStatusCode()
        //{
        //    return ShouldSerializeApiStatus();
        //}
    }
}
