namespace HT.Common.ApiMessaging
{
    /// <summary>
    /// Settings to affect default behavior(s) of the Api library
    /// </summary>
    public static class ApiSettings
    {
        /// <summary>
        /// Global setting to shape default behaviors on the ApiResponse objects
        /// </summary>
        public static class Response
        {
            public enum SerializeSwitch
            {
                /// <summary>
                /// Never serialize property or skip test
                /// </summary>
                Never=0,

                /// <summary>
                /// Only serialize property if not null and non-empty list (for lists)
                /// </summary>
                NonEmpty=1,

                /// <summary>
                /// For properties that have default behavior, only serialize if explicitly set
                /// </summary>
                SetOnly=2,

                /// <summary>
                /// Use default serialization for property
                /// </summary>
                Always=4
            }

            /// <summary>
            /// Set defaults
            /// </summary>
            static Response()
            {
                SerializeHeaderOnOk = SerializeSwitch.Always;
                SerializeHeaderApiStatus = SerializeSwitch.Always;
                SerializeHeaderActionStatus = SerializeSwitch.Always;
                SerializeHeaderIsOk = SerializeSwitch.Always;
                SerializeHeaderLinks = SerializeSwitch.NonEmpty;
                SerializeHeaderMessages = SerializeSwitch.NonEmpty;
                SerializeMessageSystemMessageOnError = SerializeSwitch.NonEmpty;
                SerializeMessageStatus = SerializeSwitch.Always;
                SerializeMessageException = SerializeSwitch.Always;
                SerializeResponseHeader = SerializeSwitch.Always;
                AutoResolveLinks = true;
            }

            /// <summary>
            /// Determine when the 'Header' field is serialized (Default: always)
            /// </summary>
            public static SerializeSwitch SerializeResponseHeader { get; set; }

            /// <summary>
            /// Return HttpStatus code within Header (Default: always)
            /// </summary>
            public static SerializeSwitch SerializeHeaderApiStatus { get; set; }

            /// <summary>
            /// Serialize 'Header' field when response is 'OK'.  (default: Always).  Some implementations might consider Headers not necessary on successful call
            /// </summary>
            public static SerializeSwitch SerializeHeaderOnOk { get; set; }

            /// <summary>
            /// Response.Header.IsOk - This same value is also on the Response itself so it might be deemed redundant (default: Always)
            /// </summary>
            public static SerializeSwitch SerializeHeaderIsOk { get; set; }

            /// <summary>
            /// Response.Header.ActionStatus - Return status of performed action code within Header (Default: always)
            /// </summary>
            public static SerializeSwitch SerializeHeaderActionStatus { get; set; }

            /// <summary>
            /// Response.Header.Links - Determines how the links collection is returned to caller
            /// </summary>
            public static SerializeSwitch SerializeHeaderLinks { get; set; }

            /// <summary>
            /// Response.Header.Messages - Determines how the messages collection is returned to caller
            /// </summary>
            public static SerializeSwitch SerializeHeaderMessages { get; set; }

            /// <summary>
            /// Message.Status - serialize Status (e.g. 4) and StatusCode (e.g. 'Warning') for each returned message
            /// </summary>
            public static SerializeSwitch SerializeMessageStatus { get; set; }

            /// <summary>
            /// Message.SystemMessage - hide/show detailed system messages if message state is reflecting an error.  May not be appropriate to serialize this to the user. (default: NotEmpty, system messages are always sent if present)
            /// </summary>
            public static SerializeSwitch SerializeMessageSystemMessageOnError { get; set; }

            /// <summary>
            /// Message.Exception - serialize any attached exceptions associated with this message (default: Always)
            /// </summary>
            public static SerializeSwitch SerializeMessageException { get; set; }

            /// <summary>
            /// Root URI stem for service (http://localhost:12345/vdir1).  Used when AutoResolving links to fully qualified URIs.  For relative URIs, leave blank.  Implementations should set this during application startup, but may be set at any time.
            /// </summary>
            public static string ServiceRoot { get; set; }

            /// <summary>
            /// Automatically resolve any tokens in Link::Location using <see cref="ApiSettings.Response.ServiceRoot"/> as the base URL (Default: true).  Set to FALSE to explicitly call msg.Header.Links.Resolve or to skip token resolution if resolution is not necessary.
            /// </summary>
            public static bool AutoResolveLinks { get; set; }
        }
    }

}
