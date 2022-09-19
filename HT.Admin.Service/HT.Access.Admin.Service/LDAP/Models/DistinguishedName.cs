using System;
using System.Collections.Generic;
using System.Text;

namespace HT.Access.Admin.Service.LDAP.Models
{
    /// <summary>
    /// Lightweight implementation around LDAP/DN/RDN   https://ldap.com/ldap-dns-and-rdns
    /// <para>
    /// RDNs are stored in least significant order first.  Thus <see cref="RdnList"/>[0] is the least significant and <see cref="RdnList"/>[len-1] is the most significant (e.g. domain)
    /// </para>
    /// </summary>
    /// <returns>This package might be a better choice: https://github.com/Ski-Dive-Dev/RFC2253/tree/master/DistinguishedNameParser</returns>
    public class DistinguishedName
    {
        private readonly List<RelativeDistinguishedName> _rdnList = new();

        public DistinguishedName()
        {
            _rdnList ??= new();
        }
        public DistinguishedName(string escapedDn) : this()
        {

            if (string.IsNullOrWhiteSpace(escapedDn))
            {
                throw new ArgumentNullException();
            }

            string[] rdnParts = escapedDn.Split(new char[] { ',' }, StringSplitOptions.TrimEntries);
            foreach (var rdnPart in rdnParts)
            {
                _rdnList.Add(new(rdnPart));
            }
        }

        public List<RelativeDistinguishedName> RdnList => _rdnList;
        public int Count => _rdnList?.Count ?? 0;
        public RelativeDistinguishedName this[int index] => Count == 0 ? null : _rdnList[index];

        /// <summary>
        /// Returns the fully qualified distinguished name (DN) that is appropriately escaped.
        /// </summary>
        public string Dn => ToString();

        /// <summary>
        /// Returns a <b>copy</b> of the first relative distinguished name (RDN).  Value is appropriately escaped.
        /// </summary>
        public RelativeDistinguishedName Rdn
        {
            get
            {
                if (_rdnList == null || _rdnList.Count == 0) return null;
                return _rdnList[0].Clone();
            }
        }

        /// <summary>
        /// Returns a <b>copy</b> of the last relative distinguished name (RDN) in the chain. Value is appropriately escaped.
        /// </summary>
        public RelativeDistinguishedName RootRdn
        {
            get
            {
                if (_rdnList == null || _rdnList.Count == 0) return null;
                return _rdnList[^1].Clone();
            }
        }

        public string ParentDn
        {
            get
            {
                if (_rdnList == null || _rdnList.Count < 2)
                {
                    return null;
                }
                var retVal = new StringBuilder();

                for (var x = 1; x < _rdnList.Count; x++)
                {
                    if (retVal.Length > 0) retVal.Append(",");
                    retVal.Append($"{_rdnList[x]}");
                }
                return retVal.ToString();
            }
        }

        public DistinguishedName Insert(string attribute, string value)
        {
            var rdn = new RelativeDistinguishedName(attribute, value);
            return Insert(rdn);
        }

        public DistinguishedName Insert(string relativeDistinguishedName)
        {
            var rdn = new RelativeDistinguishedName(relativeDistinguishedName);
            return Insert(rdn);
        }
        public DistinguishedName Insert(RelativeDistinguishedName rdn)
        {
            if (_rdnList.Count == 0)
            {
                _rdnList.Add(rdn);
            }
            else
            {
                _rdnList.Insert(0, rdn);
            }

            return this;
        }


        public override string ToString()
        {
            if (_rdnList == null || _rdnList.Count == 0) return null;
            return string.Join(",", _rdnList);
        }





        public class RelativeDistinguishedName
        {
            /// <summary>
            /// &lt;encoded&gt;,&lt;decoded&gt;
            /// </summary>
            private static readonly List<KeyValuePair<string, string>> __escapeMap = new()
            {
                new KeyValuePair<string, string>(" ", " ")
                //new KeyValuePair<string, string>(@"\\", @"\"),
                //new KeyValuePair<string, string>(@"\5c", @"\"),
                //new KeyValuePair<string, string>(@"\ ", " "),
                //new KeyValuePair<string, string>(@"\20", " "),
                //new KeyValuePair<string, string>(@"\#", "#"),
                //new KeyValuePair<string, string>(@"\23", "#"),
                //new KeyValuePair<string, string>(@"\22", "\""),
                //new KeyValuePair<string, string>(@"\\", "\""),
                //new KeyValuePair<string, string>(@"\+", "+"),
                //new KeyValuePair<string, string>(@"\2b", "+"),
                //new KeyValuePair<string, string>(@"\,", ","),
                //new KeyValuePair<string, string>(@"\2c", ","),
                //new KeyValuePair<string, string>(@"\;", ";"),
                //new KeyValuePair<string, string>(@"\3b", ";"),
                //new KeyValuePair<string, string>(@"\<", "<"),
                //new KeyValuePair<string, string>(@"\3c", "<"),
                //new KeyValuePair<string, string>(@"\>", ">"),
                //new KeyValuePair<string, string>(@"\3e", ">"),

            };

            public string Attribute { get; internal set; }
            public string Value { get; internal set; }

            internal RelativeDistinguishedName()
            {

            }
            public RelativeDistinguishedName(string rdn)
            {
                var rdnParts = rdn.Split(new char[] { '=' },
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (rdnParts == null || rdnParts.Length < 2)
                {
                    throw new ArgumentException($"RDN ('{rdn}') must have format [attribute]=[value]", nameof(rdn));
                }

                Attribute = rdnParts[0].ToLowerInvariant().Trim();
                Value = Unescape(rdnParts[1].ToLowerInvariant().Trim());
            }

            /// <summary>
            /// https://ldap.com/ldap-dns-and-rdns/
            /// </summary>
            /// <param name="nameValue"></param>
            /// <returns></returns>
            public static string Escape(string nameValue)
            {
                nameValue = nameValue?.ToLowerInvariant()?.Trim();

                if (string.IsNullOrWhiteSpace(nameValue)) return nameValue;
                foreach (var escape in __escapeMap)
                {
                    if (nameValue.Contains(escape.Value)) nameValue = nameValue.Replace(escape.Value, escape.Key);
                }
                return nameValue;
            }



            /// <summary>
            /// https://ldap.com/ldap-dns-and-rdns/
            /// </summary>
            /// <param name="nameValue"></param>
            /// <returns></returns>
            public static string Unescape(string nameValue)
            {
                nameValue = nameValue?.ToLowerInvariant()?.Trim();
                if (string.IsNullOrWhiteSpace(nameValue)) return nameValue;
                foreach (var escape in __escapeMap)
                {
                    if (nameValue.Contains(escape.Key)) nameValue = nameValue.Replace(escape.Key, escape.Value);
                }

                return nameValue;
            }

            public RelativeDistinguishedName(string attribute, string value)
            {
                Attribute = attribute.ToLowerInvariant().Trim();
                Value = Unescape(value.Trim().ToLowerInvariant());
            }

            public override string ToString()
            {
                return $"{Attribute.ToLowerInvariant()}={Escape(Value)}";
            }

            public RelativeDistinguishedName Clone(bool escapeValues = true)
            {
                return new RelativeDistinguishedName()
                {
                    Attribute = Attribute,
                    Value = escapeValues ? Escape(Value) : Value
                };
            }
        }
    }
}
