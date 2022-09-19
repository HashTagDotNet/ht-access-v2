using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace HT.Common
{
    public partial class Serialize
    {
        public class From
        {
            private static JsonSerializerSettings __jsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };


            /// <summary>
            /// Compress a set of bytes (using DeflateStream)
            /// </summary>
            /// <param name="bytesToCompress"></param>
            /// <returns></returns>
            /// <remarks>DeflateStream is better used for transmisstion and short term storage, GZip is better for long term storage</remarks>
            public static byte[] Compressed(byte[] compressedBytes)
            {
                if (compressedBytes == null) return null;
                MemoryStream input = new MemoryStream(compressedBytes);
                MemoryStream output = new MemoryStream();
                using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
                {
                    dstream.CopyTo(output);
                }
                return output.ToArray();
            }

            public static T Binary<T>(byte[] serializedBytes)
            {
                var serializedText = From.Binary(serializedBytes);
                return From.String<T>(serializedText);
            }

            public static T String<T>(string serializedObject)
            {
                return JsonConvert.DeserializeObject<T>(serializedObject, __jsonSettings);
            }
            public static string Binary(byte[] serializedBytes)
            {
                return Encoding.UTF8.GetString(serializedBytes);                
            }
        }
    }
}
