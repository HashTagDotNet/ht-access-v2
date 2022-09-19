using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using HT.Common.Reflection;
using System.Collections;
using HT.Common.Collections;
using Newtonsoft.Json;

namespace HT.Common.Diagnostics
{
	/// <summary>
	/// Serializable version of a .Net exception including all inner exceptions and public properties
	/// </summary>
	
	
	public class LogException : ICloneable
	{
		static string[] _filterList = Reflector.GetPublicPropertyNames(typeof(Exception));

		public LogException()
		{
            Properties = new SortedDictionary<string, object>();
            Data = new SortedDictionary<string, object>();
		}

		public LogException(Exception ex)
			: this()
		{
			Message = ex.Message;
			Source = ex.Source;
			StackTrace = ex.StackTrace;
			HelpLink = ex.HelpLink;
			ExceptionType = ex.GetType().FullName;
            OriginalException = ex;
			if (ex.InnerException != null)
			{
				InnerException = new LogException(ex.InnerException );
			}

			foreach (object key in ex.Data.Keys)
			{
				string keyString = key.ToString();
				Data.Add(keyString, ex.Data[key].ToString());
			}

			//-------------------------------------------------------
			// use reflection to get all public properties on the
			//	exception being examined except those defined in 
			//	_filterList (generally just those in base Exception class)
			//-------------------------------------------------------			
			var props = Reflector.GetPublicProperties(ex, _filterList);
			if (props != null)
            {
                foreach(var prop in props)
                {
                    Properties.Add(prop.Key, prop.Value);
                }
            }
			TargetSite = (ex.TargetSite == null)?"(null)":ex.TargetSite.ToString();
			ErrorCode = Reflector.GetProtectedProperty<int>("HResult", ex, default(int));
		}

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public SortedDictionary<string,object> Properties { get; set; }
        public bool ShouldSerializeProperties()
        {
            return Properties != null && Properties.Count > 0;
        }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public LogException InnerException { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Source { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string StackTrace { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string HelpLink { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string TargetSite { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SortedDictionary<string,object> Data { get; set; }
        public bool ShouldSerializeData()
        {
            return Data != null && Data.Count > 0;
        }
        /// <summary>
        ///  A coded value that is assigned to a specific exception. Often the HRESULT from the attached exception 
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int ErrorCode { get; set; }

        /// <summary>
        /// .Net data type of exception
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ExceptionType { get; set; }

		/// <summary>
		/// Get's the innermost exception or a reference to this instance if there are no inner exceptions
		/// </summary>
		/// <returns></returns>
		public LogException GetBaseException()
		{
			var retEx = this;
			while (retEx.InnerException != null)
			{
				retEx = retEx.InnerException;
			}
			return retEx;
		}
        [JsonIgnore]
        public Exception OriginalException { get; set; }

        [JsonIgnore]
        public LogException BaseException => GetBaseException();
        
		public object Clone()
		{
			var retEx = new LogException()
			{
				ErrorCode = this.ErrorCode,
				ExceptionType = this.ExceptionType,
				HelpLink = this.HelpLink,
				Message = this.Message,
				Source = this.Source,
				StackTrace = this.StackTrace,
				TargetSite = this.TargetSite
			};
			if (InnerException != null)
			{
				retEx.InnerException = (LogException)InnerException.Clone();
			}
			if (Data != null && Data.Count > 0)
			{
				foreach (var item in Data)
				{
					retEx.Data.Add(item.Key,item.Value);
				}
			}

			if (Properties != null && Properties.Count > 0)
			{
				foreach (var prop in Properties)
				{
					retEx.Properties.Add(prop.Key,prop.Value);
				}
			}
			return retEx;
		}
		
		public override string ToString()
		{
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            });
		}
        
	}
}
