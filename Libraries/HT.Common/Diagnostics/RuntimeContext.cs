using HT.Common.Collections;
using HT.Common.Reflection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HT.Common.Diagnostics
{
    /// <summary>
    /// Common properties that uniquely identifies an running instance of an application.
    /// These properties are used to enhance the messages that are written to logging store
    /// </summary>
    /// <remarks>This class started out in Logging but was promoted to this library since other systems started needing reference to it</remarks>
    public class RuntimeContext : Dictionary<string, string>
    {
        private static string __hostName = Environment.MachineName;
        private static string __runtimeId = Guid.NewGuid().ToString().Substring(0, 8).ToLower();
        public static RuntimeContext Create<THost>(Action<RuntimeContext> options = null) where THost : class
        {
            var retVal = new RuntimeContext(options);
            retVal.EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                ?? Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production";
            var t = typeof(THost);
            retVal.ApplicationName = t.ApplicationName();
            retVal.ApplicationVersion = t.FileVersion();
            retVal.ProductName = t.Product();
            options?.Invoke(retVal);
            _ = retVal.ApplicationUri;
            _ = retVal.SourceUri;

            return retVal;
        }
        public RuntimeContext(Action<RuntimeContext> options = null)
        {
            MachineName = __hostName;
            InstanceName = __runtimeId;
            AppInsightsKey = Environment.GetEnvironmentVariable("appinsights_instrumentationkey");
            options?.Invoke(this);
            //force URI side effects 
            _ = SourceUri;

        }

        [JsonIgnore]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string AppInsightsKey { get { return this.GetValue("aiKey"); } set { this.SetValue("aiKey", value); } }

        [JsonIgnore]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string ProductName { get { return this.GetValue("ProductName"); } set { this.SetValue("ProductName", value); } }

        [JsonIgnore]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string ApplicationName { get { return this.GetValue("AppName"); } set { this.SetValue("AppName", value); } }

        [JsonIgnore]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string EnvironmentName { get { return this.GetValue("EnvironmentName"); } set { this.SetValue("EnvironmentName", value); } }

        [JsonIgnore]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string MachineName { get { return this.GetValue("MachineName"); } set { this.SetValue("MachineName", value); } }

        [JsonIgnore]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string Module { get { return this.GetValue("Module"); } set { this.SetValue("Module", value); } }

        [JsonIgnore]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceName { get { return this.GetValue("ServiceInstanceId"); } set { this.SetValue("ServiceInstanceId", value); } }

        //[JsonIgnore]
        //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        //public string RuntimeId { get { return this.GetValue("InstanceId"); } set { this.SetValue("InstanceId", value); } }

        [JsonIgnore]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string ApplicationVersion { get { return this.GetValue("ApplicationVersion"); } set { this.SetValue("ApplicationVersion", value); } }

        /// <summary>
        /// Returns a fully qualified path the identifies this context including instance, and runtime identifiers
        /// </summary>
        [JsonIgnore]
        public string SourceUri
        {
            get
            {
                var items = new[] { ApplicationUri, InstanceName };
                var name = string.Join("/", items.Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
                this.SetValue("SourceUri", name);
                return name;
            }
        }

        /// <summary>
        /// Returns product/applicationname/module that identifies the application.  Used in filter selection and message enhancement
        /// </summary>
        [JsonIgnore]
        public string ApplicationUri
        {
            get
            {
                var items = new[] { ProductName, Module, ApplicationName };
                var uri = string.Join("/", items.Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
                this.SetValue("AppUri", uri);
                return uri;
            }
        }

        /// <summary>
        /// Use this method to force Uri methods to recaculate immediately before serializing
        /// </summary>
        /// <param name="context"></param>
        /// <remarks>https://www.newtonsoft.com/json/help/html/SerializationCallbacks.htm</remarks>
        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            //force URI side effects //TBD does this work in .net Core 2.2 with MSFTs proprietary JSON serializer?
            _ = SourceUri;
        }

    }
}
