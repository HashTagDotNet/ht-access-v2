using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HT.Common.ApiMessaging
{
    /// <summary>
    /// Provides a common set of properties for all responses
    /// </summary>
    public class ApiResponse
    {

        public ApiResponse()
        {
           
        }

        /// <summary>
        /// Status and message(s) service wishes to return to caller. In some 
        /// environments (interal server-to-server, DEV) there
        /// might be hydrated with implementation specific version(s).  In
        /// PROD much information might be converted into reference links,
        /// deprecated, or otherwise obfuscated.
        /// </summary>
        /// <remarks>
        /// Some implementations might not return header (e.g. Header == null) if
        /// request completes with a success code.  This is NOT recommended but
        /// possible.  It is better to return a header with the known IsOk property
        /// set so consumers always know return results
        /// </remarks>
        [JsonProperty("header", NullValueHandling = NullValueHandling.Ignore)]
        public ApiResponseHeader Header { get; set; } = new ApiResponseHeader();

        /// <summary>
        /// Data to return to client (may be null).  Might be validation messages, 
        /// tabular data, or any service specific format.
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public virtual object Data { get; set; }

        /// <summary>
        /// Miscellaneous settings which the service wishes to return to caller (e.g timings, 
        /// api version, server, total records processed, etc.).  Consumer of
        /// this response shouldn't normally depend on these values.  Often null in PROD environments
        /// </summary>
        [JsonProperty("meta", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Meta { get; set; }



        //public bool ShouldSerializeHeader()
        //{
        //    switch (ApiSettings.Response.SerializeResponseHeader)
        //    {
        //        case ApiSettings.Response.SerializeSwitch.Never: return false;
        //        case ApiSettings.Response.SerializeSwitch.NonEmpty: return shouldSerializeOnOk(true);
        //        case ApiSettings.Response.SerializeSwitch.SetOnly: return shouldSerializeOnOk(true);
        //        case ApiSettings.Response.SerializeSwitch.Always: return shouldSerializeOnOk(true);
        //    }
        //    return true; //default to serialize
        //}

        //private bool shouldSerializeOnOk(bool currentValue)
        //{
        //    switch (ApiSettings.Response.SerializeHeaderOnOk)
        //    {
        //        case ApiSettings.Response.SerializeSwitch.Never: return IsOk==true ? false:currentValue;
        //        case ApiSettings.Response.SerializeSwitch.NonEmpty: return true;
        //        case ApiSettings.Response.SerializeSwitch.SetOnly: return true;
        //        case ApiSettings.Response.SerializeSwitch.Always: return true;
        //    }
        //    return true; //default to serialize
        //}
        //public bool ShouldSerializeIsOk()
        //{
        //    switch (ApiSettings.Response.SerializeHeaderIsOk)
        //    {
        //        case ApiSettings.Response.SerializeSwitch.Never: return false;
        //        case ApiSettings.Response.SerializeSwitch.NonEmpty: return true;
        //        case ApiSettings.Response.SerializeSwitch.SetOnly: return true;
        //        case ApiSettings.Response.SerializeSwitch.Always: return true;
        //    }
        //    return true; //default to serialize
        //}

    }

    /// <summary>
    /// Creates a common set of response properties and a strongly typed 'Body' field that contains a returned object
    /// </summary>
    /// <typeparam name="TResponseData">Type of body content</typeparam>
    public class ApiResponse<TResponseData>:ApiResponse 
    {
        /// <summary>
        /// Default constructor and attempts to create an instance of TBody. Null if fails.
        /// </summary>
        public ApiResponse()
        {
            if (typeof(TResponseData).IsPrimitive == true || typeof(TResponseData).FullName == "System.String")
            {
                Data = default(TResponseData);
            }
            else
            {
                try
                {
                    Data = Activator.CreateInstance<TResponseData>();
                }
                catch
                {
                    //in these cases caller must initialize field;
                 //   throw new InvalidOperationException(string.Format("Unable to create an instance of '{0}'. {1}", typeof(T).FullName, ex.Expand()));
                }
            }
        }

        /// <summary>
        /// Initializes response with an error
        /// </summary>
        /// <param name="ex">Exception to be associated with this response</param>
        public ApiResponse(Exception ex, string displayMessage=null, params object[] args):base()
        {            
            Catch(ex,displayMessage,args);
        }

        /// <summary>
        /// Adds an error to this message.  More than exception can be associated with this response
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public ApiEventBuilder Catch(Exception ex, string titleMessage = null, params object[] args)
        {
            return Header.Events.Add().Catch(ex).Title(titleMessage,args);            
        }

        public ApiResponse(TResponseData responseData)
        {
            this.Data = responseData;
        }

        /// <summary>
        /// Strongly typed data to return to client (may be null)
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public new TResponseData Data
        {
            get
            {
                return (TResponseData) base.Data;
            }
            set
            {
                base.Data = value;
            }
        }

    }
}
