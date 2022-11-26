using System.Collections.Generic;
using System.Net;
using HT.Common.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HT.Common.ApiMessaging
{
    /// <summary>
    /// Represents a bit of reponse metadata text this API wishes to include in the response.  Normally do not use this class for returning client data, instead return client data within the body of the message.  ApiMessages are helpful for returning
    /// system messages and error messages or other out-of-band contextual information (e.g. "Duplicate record exception on table FOO_2","You are not authorized to eat french-fries")
    /// </summary>
    /// <remarks>Similar to IDataErrorInfo implmentations with extensions</remarks>
    /// <example>
    /// {
    ///     "id": "3433l213-2342-sfd",
    ///     "links":[
    ///         "details":"https://myerrorwebsite/errors/345-sdfas-2222"
    ///     ],
    ///     "status": 200,
    ///     "statusCode" : "OK",
    ///     "code": "APP-23-DSG-001",
    ///     "eventId": "30211",
    ///     "title":"Call service desk with reference #: '3433l213-2342-sfd'",
    ///     "detail":"...complete-call-stack, execution context, and machine parameters...",
    ///     "severity": 3,
    ///     "severityCode": "Error",
    ///     "isOk": false
    /// }
    /// </example>
    public class ApiEvent
    {
        /// <summary>
        /// Unique identifier for this instance of this message or occurance of problem.  
        /// Another event with the same 'Code/EventId' results in a new ID.  This can be 
        /// helpful if this event is writting to a common logging store
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string ID { get; set; }

        /// <summary>
        /// link(s) that leads to further details about this particular occurence of the problem.  For example to a common ELMAH store.
        /// Note: For many implementations this property will be NULL.
        /// </summary>
        [JsonProperty("links", NullValueHandling = NullValueHandling.Ignore)]
        public SortedDictionary<string, string> Links { get; set; }

        /// <summary>
        /// Recommended HTTP status code the service wants to return to caller.  Possible values are determined by request/response.  Often HTTP status code
        /// </summary>
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public HttpStatusCode HttpStatus { get; set; } = HttpStatusCode.OK;

        /// <summary>
        /// String version of Status (e.g. OK, FileNotFound)
        /// </summary>
        [JsonProperty("statusCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public HttpStatusCode HttpStatusCode
        {
            get
            {
                return HttpStatus;
            }
            set
            {
                HttpStatus = value;
            }
        }

        /// <summary>
        /// (optional) System wide identifier (e.g. AP10001, HR92922, ERR.10) that identifies this message.  Often message code or error code (e.g. ERR-1010).        
        /// Often used in routing, response strategies, and especially localization as a key to resource files.
        /// </summary>
        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public object Code { get; set; }

        /// <summary>
        /// Application specific unique id for this message.  There SHOULD be only one event of this id 
        /// in the system (and perhaps in API ecosystem). (e.g. 20101 - Application accepted).  Often
        /// used with database log stores, semantic logging (Event Tracing Windows, Enterprise Library, .Net Core logging)
        /// <para>
        /// Some systems might use EventId as a resource identifier to lookup localized versions of message
        /// </para>
        /// </summary>
        /// <remarks>
        /// see: https://msdn.microsoft.com/en-us/library/windows/desktop/dd392330(v=vs.85).aspx
        /// </remarks>
        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public int EventId { get; set; }

        /// <summary>
        /// a short, human-readable summary of the problem that 
        /// SHOULD NOT change from occurrence to occurrence of the problem, except for purposes of localization.
        /// This content IS often displayed to the user
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        /// <summary>
        /// a human-readable explanation specific to this occurrence of the problem / message.
        /// Often used in "more info" scenarios
        /// </summary>
        [JsonProperty("detail", NullValueHandling = NullValueHandling.Ignore)]
        public string Detail { get; set; }

        private string _systemMessage = null;
        /// <summary>
        /// Detailed system level information (error messages, reference numbers, etc) but not generally displayed to user.
        /// Often null in PROD environments, or might contain stack trace or other informaiton in DEV environments.
        /// This content MAY contain technical details and is often NOT displayed to the user and
        /// used in development and diagnostic scenarios.
        /// </summary>
        [JsonProperty("systemMessage",NullValueHandling = NullValueHandling.Ignore)]
        public string SystemMessage
        {
            get
            {
                if (_systemMessage == null)
                {
                    if (Exception != null)
                    {
                        return Exception.Message;
                    }
                }
                return _systemMessage;
            }
            set
            {
                _systemMessage = value;
            }
        }

        /// <summary>
        /// Any contextural information to assoicate with this information (e.g. id number, state)
        /// or where the source of the error occured. Often field name or text connecting an field or property to a message.
        /// Format of this field is service call specific but is usually a number or string.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public string Reference { get; set; }

        /// <summary>
        /// Serialized exception details associated with this message.  Often used in core-to-core calls where 
        /// application developers might need service error details to help troubleeshoot service. In PROD
        /// environments service MAY convert exception into links and nullify this property.
        /// </summary>
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public LogException Exception { get; set; }

        private ApiEventStatus? _messageStatus;
        /// <summary>
        /// Numeric Level (e.g. Info=2, Warning=3, Error=4, Critical=5) service wishes to return to caller.
        /// Determines how important the creator of this message considers it. (default: Info)  If not explicitly set returns Error if Exception is set, otherwise return Info
        /// Some callers (or APIs) might use this indicator instead of HTTP status as success indicator.
        /// (default: None)
        /// </summary>
        [JsonProperty("severity")]
        public ApiEventStatus MessageStatus
        {
            get
            {
                if (_messageStatus.HasValue)
                {
                    return _messageStatus.Value;
                }

                if (Exception != null || HttpStatusCode >= HttpStatusCode.BadRequest)  //no status code assigned but exception has been assigned
                {
                    return ApiEventStatus.Error;
                }

                return ApiEventStatus.Info; //no default can be determined from other fields so return standard default

            }
            set
            {
                _messageStatus = value;
            }
        }

        /// <summary>
        /// Text of severity (e.g. Info, Warning, Error, Critical) service wishes to return to caller.
        /// </summary>
        [JsonProperty("serverityCode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ApiEventStatus MessageStatusCode
        {
            get
            {
                return MessageStatus;
            }
            set
            {
                MessageStatus = value;
            }
        }

        bool? _isOk;
        /// <summary>
        ///Convenience property to check for errors (no results IsOk==true)
        /// </summary>
        [JsonProperty("isOk")]
        public bool IsOk
        {
            get
            {
                if (_isOk.HasValue)
                {
                    return _isOk.Value;
                }
                if (MessageStatus > ApiEventStatus.Warning)
                {
                    return false;
                }
                if (HttpStatus >= HttpStatusCode.BadRequest)
                {
                    return false;
                }
                return true;
            }
            set
            {
                _isOk = value;
            }
        }

        [JsonIgnore]
        internal int CombinedSeverity
        {
            get
            {
                var severity = ((int) HttpStatus) * 10;

                switch (MessageStatus)
                {
                    case ApiEventStatus.None:
                        severity += 1;
                        break;
                    default:
                        severity += ((int) MessageStatus) + 1;
                        break;
                }
                return severity;
            }
        }

        ///// <summary>
        ///// Json.Net serializing instructions
        ///// </summary>
        ///// <returns></returns>
        //public bool ShouldSerializeException()
        //{
        //    switch (ApiSettings.Response.SerializeMessageException)
        //    {
        //        case ApiSettings.Response.SerializeSwitch.Never: return false;
        //        case ApiSettings.Response.SerializeSwitch.NonEmpty:
        //        case ApiSettings.Response.SerializeSwitch.SetOnly: return Exception != null;
        //        case ApiSettings.Response.SerializeSwitch.Always: return true;
        //    }
        //    return true; //default to serialize
        //}

        ///// <summary>
        ///// Json.Net serializing instructions
        ///// </summary>
        ///// <returns></returns>
        //public bool ShouldSerializeStatus()
        //{
        //    switch (ApiSettings.Response.SerializeMessageStatus)
        //    {
        //        case ApiSettings.Response.SerializeSwitch.Never: return false;
        //        case ApiSettings.Response.SerializeSwitch.NonEmpty: return _msgStatus != null;
        //        case ApiSettings.Response.SerializeSwitch.SetOnly: return _msgStatus != null;
        //        case ApiSettings.Response.SerializeSwitch.Always: return true;
        //    }
        //    return true; //default to serialize
        //}
        ///// <summary>
        ///// Json.Net serializing instructions
        ///// </summary>
        ///// <returns></returns>
        //public bool ShouldSerializeSystemMessage()
        //{
        //    switch (ApiSettings.Response.SerializeMessageSystemMessageOnError)
        //    {
        //        case ApiSettings.Response.SerializeSwitch.Never: if (IsOk == false) return false; break;
        //        case ApiSettings.Response.SerializeSwitch.NonEmpty: if (IsOk == false) return true; break;
        //        case ApiSettings.Response.SerializeSwitch.SetOnly: if (IsOk == false && _systemMessage != null) return true; break;
        //        case ApiSettings.Response.SerializeSwitch.Always: return true;
        //    }
        //    return true; //default to serialize
        //}
        ///// <summary>
        ///// Json.Net serializing instructions
        ///// </summary>
        ///// <returns></returns>
        //public bool ShouldSerializeStatusCode()
        //{
        //    return ShouldSerializeStatus();
        //}

        //private ApiHeaderMessageBuilder _builder;
        ///// <summary>
        ///// Returns reference to internal fluent builder for this message.
        ///// </summary>
        //[JsonIgnore]
        //public ApiHeaderMessageBuilder _
        //{
        //    get
        //    {
        //        if (_builder == null)
        //        {
        //            _builder = new ApiHeaderMessageBuilder(this);
        //        }
        //        return _builder;
        //    }
        //}

        //public static ApiHeaderMessageBuilder Create()
        //{
        //    ApiEvent msg = new ApiEvent();
        //    return msg._;
        //}

        //public class ApiHeaderMessageBuilder
        //{
        //    private ApiEvent _msg;
        //    internal ApiHeaderMessageBuilder(ApiEvent msg)
        //    {
        //        _msg = msg;
        //    }

        //    public ApiHeaderMessageBuilder Catch(Exception ex=null)
        //    {
        //        if (ex == null)
        //        {
        //            _msg.Exception = null;
        //        }
        //        else
        //        {
        //            _msg.Exception = new LogException(ex);
        //        }

        //        return this;
        //    }

        //    public ApiHeaderMessageBuilder Reference(string referenceText, params object[] args)
        //    {
        //        if (referenceText == null)
        //        {
        //            _msg.Reference = referenceText;
        //        }
        //        else
        //        {
        //            _msg.Reference = string.Format(referenceText, args);
        //        }
        //        return this;
        //    }
        //    public ApiHeaderMessageBuilder SystemMessage(string message, params object[] args)
        //    {
        //        if (message == null)
        //        {
        //            _msg.SystemMessage = null;
        //        }
        //        else
        //        {
        //            _msg.SystemMessage = string.Format(message, args);
        //        }
        //        return this;
        //    }
        //    public ApiHeaderMessageBuilder DisplayMessage(string message, params object[] args)
        //    {
        //        if (message == null)
        //        {
        //            _msg.DisplayMessage = null;
        //        }
        //        else
        //        {
        //            _msg.DisplayMessage = string.Format(message, args);
        //        }
        //        return this;
        //    }
        //    public ApiHeaderMessageBuilder MessageCode(string code, params object[] args)
        //    {
        //        if (code == null)
        //        {
        //            _msg.MessageCode = null;
        //        }
        //        else
        //        {
        //            _msg.MessageCode = string.Format(code, args);
        //        }
        //        return this;
        //    }
        //    public ApiHeaderMessageBuilder Status(ApiMessageStatus status)
        //    {
        //        _msg._msgStatus = status;
        //        return this;
        //    }

        //    public ApiEvent Message
        //    {
        //        get
        //        {
        //            return _msg;
        //        }
        //    }
        //}
    }

}
