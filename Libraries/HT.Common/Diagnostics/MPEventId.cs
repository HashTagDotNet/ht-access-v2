using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HT.Common.Diagnostics
{
    /// <summary>
    /// A int-string tuple to represent an id &amp; code; either of which is optional.  Default id is '0', code=null if none specified.
    /// </summary>
    /// <remarks>Called 'MPEventId' instead of EventId to avoid name collision with Msft.Extensions.Logging.EventId</remarks>
    public class MPEventId
    {
        public static MPEventId NewId(int id, string code = null)
        {
            return new MPEventId(id, code);
        }
        public static MPEventId NewId(string code)
        {
            return new MPEventId(code);
        }
        public static implicit operator MPEventId(int i)
        {
            return new MPEventId(i);
        }

        public static implicit operator MPEventId(string code)
        {
            return new MPEventId(code);
        }
        public static implicit operator MPEventId(EventId id)
        {
            return new MPEventId(id.Id, id.Name);
        }

        public static explicit operator EventId(MPEventId id)
        {
            return new EventId(id.Id, id.Code);
        }

        public static bool operator ==(MPEventId left, MPEventId right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right)) return true;
            if (ReferenceEquals(null, left) && !ReferenceEquals(null, right)) return false;
            if (!ReferenceEquals(null, left) && ReferenceEquals(null, right)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(MPEventId left, MPEventId right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right)) return false;
            if (ReferenceEquals(null, left) && !ReferenceEquals(null, right)) return true;
            if (!ReferenceEquals(null, left) && ReferenceEquals(null, right)) return true;

            return !left.Equals(right);
        }
        public MPEventId()
        {

        }
        public MPEventId(int id, string code = null)
        {
            Id = id;
            Code = code;
        }


        public MPEventId(string code)
        {
            Code = code;
            Id = 0;
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Code { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Code)) // no code provided
            {
                if (Id != 0)
                {
                    return Id.ToString();
                }
                return null;
            }
            // code has been provided
            if (Id == 0)
            {
                return Code;
            }
            return $"{Code} ({Id})";
        }

        public bool Equals(MPEventId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (Id != other.Id) return false;
            return string.Compare(Code, other.Code, true) == 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is MPEventId eventId && Equals(eventId);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
