using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace HT.Common
{
    public partial class Serialize
    {
        public class To
        {
            private static JsonSerializerSettings __jsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };


            /// <summary>
            /// Makes a colon delmited key-value pair of JSON objects.  Based on .Net JSON Configuration file parser.  https://github.com/aspnet/Configuration/blob/master/src/Config.Json/JsonConfigurationFileParser.cs
            /// </summary>
            /// <param name="objectToSerialize"></param>
            /// <returns></returns>
            /// <remarks>https://github.com/aspnet/Configuration/blob/master/src/Config.Json/JsonConfigurationFileParser.cs</remarks>
            public static IDictionary<string, string> FlatJson(object objectToSerialize)
            {
                if (objectToSerialize == null) return new SortedDictionary<string, string>();
                return new ObjectFlattener().Parse(objectToSerialize);
            }

            public static IDictionary<string, string> FlatJson(object objectToSerialize, JsonSerializerSettings settings)
            {
                if (objectToSerialize == null) return new SortedDictionary<string, string>();
                var retVal = new ObjectFlattener().Parse(objectToSerialize, settings);
                return retVal;
            }

            public static IDictionary<string, string> FlatJson(string jsonObjectString)
            {
                if (string.IsNullOrWhiteSpace(jsonObjectString)) return new Dictionary<string, string>();
                var streamToParse = new MemoryStream(Encoding.UTF8.GetBytes(jsonObjectString));
                return new ObjectFlattener().Parse(streamToParse);
            }
            public static IDictionary<string, string> FlatJson(string jsonObjectString, JsonSerializerSettings settings)
            {
                if (string.IsNullOrWhiteSpace(jsonObjectString)) return new Dictionary<string, string>();
                var streamToParse = new MemoryStream(Encoding.UTF8.GetBytes(jsonObjectString));
                return new ObjectFlattener().Parse(streamToParse, settings);
            }

            /// <summary>
            /// Convert an object to bytes after first serializing object to JSON. See remarks for more info.
            /// </summary>
            /// <param name="objectToSerialize"></param>
            /// <returns>Null if <paramref name="objectToSerialize"/> is null or byte array</returns>
            /// <remarks>Serializing to JSON (instead of straight binary serialization) allows objects to be deserialized to a different class thus breaking binary binding on serialized data</remarks>
            public static byte[] Binary(object objectToSerialize)
            {
                if (objectToSerialize == null) return null;
                var jsonString = JsonConvert.SerializeObject(objectToSerialize, __jsonSettings);
                return Encoding.UTF8.GetBytes(jsonString);
            }

            public static string String(object objectToSerialize)
            {
                if (objectToSerialize == null) return null;
                return JsonConvert.SerializeObject(objectToSerialize, __jsonSettings);
            }

            /// <summary>
            /// Compress a set of bytes (using DeflateStream)
            /// </summary>
            /// <param name="bytesToCompress"></param>
            /// <returns>DeflateStream is better used for transmisstion and short term storage, GZip is better for long term storage</returns>
            public static byte[] Compressed(byte[] bytesToCompress)
            {
                if (bytesToCompress == null) return null;
                MemoryStream output = new MemoryStream();
                using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
                {
                    dstream.Write(bytesToCompress, 0, bytesToCompress.Length);
                }
                return output.ToArray();
            }


            private class ObjectFlattener
            {
                private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                private readonly Stack<string> _context = new Stack<string>();
                private string _currentPath;

                private JsonTextReader _reader;

                public IDictionary<string, string> Parse(object objectToFlatten)
                {
                    return Parse(objectToFlatten,__jsonSettings);
                }
                public IDictionary<string, string> Parse(object objectToFlatten, JsonSerializerSettings serializerSettings)
                {
                    var objectString = JsonConvert.SerializeObject(objectToFlatten, Formatting.Indented, serializerSettings);
                    var streamToParse = new MemoryStream(Encoding.UTF8.GetBytes(objectString));
                    return new ObjectFlattener().Parse(streamToParse);
                }
                public IDictionary<string, string> Parse(Stream input)
                {
                    _data.Clear();
                    _reader = new JsonTextReader(new StreamReader(input));
                    _reader.DateParseHandling = DateParseHandling.None;

                    var jsonConfig = JObject.Load(_reader);

                    VisitJObject(jsonConfig);

                    return _data;
                }

                private void VisitJObject(JObject jObject)
                {
                    foreach (var property in jObject.Properties())
                    {
                        EnterContext(property.Name);
                        VisitProperty(property);
                        ExitContext();
                    }
                }

                private void VisitProperty(JProperty property)
                {
                    VisitToken(property.Value);
                }

                private void VisitToken(JToken token)
                {
                    switch (token.Type)
                    {
                        case JTokenType.Object:
                            VisitJObject(token.Value<JObject>());
                            break;

                        case JTokenType.Array:
                            VisitArray(token.Value<JArray>());
                            break;

                        case JTokenType.Integer:
                        case JTokenType.Float:
                        case JTokenType.String:
                        case JTokenType.Boolean:
                        case JTokenType.Bytes:
                        case JTokenType.Raw:
                        case JTokenType.Null:
                            VisitPrimitive(token.Value<JValue>());
                            break;

                        default:
                            throw new FormatException($"Unsupports JsonToken Type: {_reader.TokenType} Path: {_reader.Path} LineNumber: {_reader.LineNumber} LinePostion: {_reader.LinePosition}");
                    }
                }

                private void VisitArray(JArray array)
                {
                    for (int index = 0; index < array.Count; index++)
                    {
                        EnterContext(index.ToString());
                        VisitToken(array[index]);
                        ExitContext();
                    }
                }

                private void VisitPrimitive(JValue data)
                {
                    var key = _currentPath;

                    if (_data.ContainsKey(key))
                    {
                        throw new FormatException($"Key '{key}' already exists");
                    }
                    _data[key] = data.ToString(CultureInfo.InvariantCulture);
                }

                private void EnterContext(string context)
                {
                    _context.Push(context);
                    var items = _context.Reverse();
                    if (items == null) return;
                    var s = "";
                    foreach (var item in items)
                    {
                        if (s.Length > 0) s += ":";
                        s += item;
                    }
                    _currentPath = s;
                    //_currentPath = ConfigurationPath.Combine(_context.Reverse());
                }

                private void ExitContext()
                {
                    _context.Pop();
                    var items = _context.Reverse();
                    if (items == null) return;
                    var s = "";
                    foreach (var item in items)
                    {
                        if (s.Length > 0) s += ":";
                        s += item;
                    }
                    _currentPath = s;
                    //_currentPath = ConfigurationPath.Combine(_context.Reverse());
                }
            }
        }
    }
}
