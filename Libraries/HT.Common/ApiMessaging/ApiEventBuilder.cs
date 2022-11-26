using System;
using System.Collections.Generic;
using System.Net;
using HT.Common.Diagnostics;

namespace HT.Common.ApiMessaging
{
    public class ApiEventBuilder
    {
        private ApiEvent _msg;
        internal ApiEventBuilder(ApiEvent msg)
        {
            _msg = msg;
        }

        public ApiEventBuilder Catch(Exception ex = null)
        {
            if (ex == null)
            {
                _msg.Exception = null;
            }
            else
            {
                _msg.Exception = new LogException(ex);
            }

            return this;
        }


        public ApiEventBuilder Reference(string referenceText, params object[] args)
        {
            if (referenceText == null)
            {
                _msg.Reference = referenceText;
            }
            else
            {
                _msg.Reference = string.Format(referenceText, args);
            }
            return this;
        }
        public ApiEventBuilder SystemMessage(string message, params object[] args)
        {
            if (message == null)
            {
                _msg.SystemMessage = null;
            }
            else
            {
                _msg.SystemMessage = string.Format(message, args);
            }
            return this;
        }
        public ApiEventBuilder DetailMessage(string message, params object[] args)
        {
            if (message == null)
            {
                _msg.Detail = null;
            }
            else
            {
                _msg.Detail = string.Format(message, args);
            }
            return this;
        }
        public ApiEventBuilder Title(string message, params object[] args)
        {
            if (message == null)
            {
                _msg.Title = null;
            }
            else
            {
                _msg.Title = string.Format(message, args);
            }
            return this;
        }
        public ApiEventBuilder MessageCode(string code, params object[] args)
        {
            if (code == null)
            {
                _msg.Code = null;
            }
            else
            {
                _msg.Code = string.Format(code, args);
            }
            return this;
        }
        public ApiEventBuilder Status(ApiEventStatus status)
        {
            _msg.MessageStatus = status;
            return this;
        }
        public ApiEventBuilder HttpStatus(HttpStatusCode code)
        {
            _msg.HttpStatus = code;
            return this;
        }
        public ApiEventBuilder EventId(int eventId)
        {
            _msg.EventId = eventId;
            return this;
        }
        public ApiEventBuilder Link(string linkName, string linkValue)
        {
            if (_msg.Links == null)
            {
                _msg.Links = new SortedDictionary<string, string>();
            }
            _msg.Links[linkName] = linkValue;
            return this;
        }

        public ApiEvent Message
        {
            get
            {
                return _msg;
            }
        }
    }
}
