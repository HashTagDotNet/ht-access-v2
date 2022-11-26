using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace HT.Common.ApiMessaging
{
 
    /// <summary>
    /// List of messages associated with an API request-response pair
    /// </summary>
    public class ApiEvents : List<ApiEvent>
    {
        private object _listLock = new Object();

        /// <summary>
        /// Get a named group of messages
        /// </summary>
        /// <param name="messageReference">Name of field or group of messages.  Often used in returning field level validation messages.  Empty/Null values are considered response level messages</param>
        /// <returns></returns>
        [JsonIgnore]
        public List<ApiEvent> this[string messageReference]
        {
            get
            {
                lock (_listLock)
                {
                    return this.Where(item => string.Compare(item.Reference, messageReference) == 0).ToList();
                }
            }
            set
            {
                lock (_listLock)
                {
                    base.AddRange(value);
                }
            }
        }

        /// <summary>
        /// True if all messages are not in 'Warning' or more severe state
        /// </summary>
        [JsonIgnore]
        public bool IsOk
        {
            get
            {
                var firstErrorMessage = this.FirstOrDefault(item => !item.IsOk);
                return firstErrorMessage == null;
            }
        }

        /// <summary>
        /// Exposes a fluent builder for adding a message to this collection.  NOTE:  calling 'Add' without invoking additional methods will add an empty record
        /// </summary>
        /// <returns></returns>
        public ApiEventBuilder Add()
        {
            lock (_listLock)
            {
                var msg = new ApiEvent();
                base.Add(msg);
                return new ApiEventBuilder(msg);
            }
        }

        /// <summary>
        /// Convenience method for Add()
        /// </summary>
        /// <param name="titleMessage"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ApiEventBuilder Add(string titleMessage, params object[] args)
        {
            return Add().Title(titleMessage, args);
        }

        /// <summary>
        /// Convenience method for Add()
        /// </summary>
        /// <param name="titleMessage"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ApiEventBuilder Add(ApiEventStatus status, string titleMessage=null, params object[] args)
        {
            return Add().Title(titleMessage, args).Status(status);
        }

        /// <summary>
        /// Convenience method for Add()
        /// </summary>
        /// <param name="titleMessage"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ApiEventBuilder Add(Exception ex, string titleMessage = null, params object[] args)
        {
            return Add().Title(titleMessage, args).Catch(ex);
        }
    }
}
