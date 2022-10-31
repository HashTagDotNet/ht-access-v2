using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HT.Access.Admin.Service.LDAP.Models
{
    public class LdifCommandBase
    {
        public const string COMMAND_ADD = "add";
        public const string COMMAND_DELETE = "delete";
        public const string COMMAND_MODIFY = "modify";
        public const string COMMAND_MODIFYDN = "moddn";
        public const string COMMAND_MODIFYRDN = "modrnd";
        public const string OBJECT_CLASS = "objectClass";
        public const string OBJECT_CLASS_TOP = "top";
        public const string COMMAND_CHANGE_TYPE = "changetype";

        private readonly List<KeyValuePair<string, string>> _lines = new();

        public DnChangeType ChangeType { get; private set; }
        public DistinguishedName Dn { get; private set; }
        public DistinguishedName ParentDn { get; private set; }
        public LdifStatusCode ExecuteStatus { get; set; }
        public string ExecuteStatusMessage { get; set; }
        public List<KeyValuePair<string, string>> CommandLines => _lines;
        
        public LdifCommandBase(DistinguishedName dn, DnChangeType changeType)
        {
            Dn = dn;
            ChangeType = changeType;
            ParentDn = dn.ParentDn == null ? null : new DistinguishedName(dn.ParentDn);

            string commandString = changeType switch
            {
                DnChangeType.Add => COMMAND_ADD,
                DnChangeType.Delete => COMMAND_DELETE,
                DnChangeType.Modify => COMMAND_MODIFY,
                DnChangeType.ModifyDn => COMMAND_MODIFYDN,
                DnChangeType.ModifyRdn => COMMAND_MODIFYRDN,
                _ => throw new InvalidEnumArgumentException()
            };

            addLine("dn",dn.ToString());
            addLine(COMMAND_CHANGE_TYPE,commandString);
        
        }
        
        public LdifCommandBase AddAttribute(string attribute, params string[] values)
        {
            if (values == null || values.Length == 0)
            {
                addLine(attribute);
            }
            else
            {
                foreach (var value in values)
                {
                    addLine(attribute, value);
                }
            }
            return this;
        }

        private void addLine(string key, string value = null)
        {
            _lines.Add(new KeyValuePair<string, string>(key, value));
        }

        internal string Build()
        {
            var sb = new StringBuilder();
            foreach (var line in _lines)
            {
                if (sb.Length > 0)
                {
                    sb.Append($"{Environment.NewLine}{line.Key}: {line.Value}");
                }
                else
                {
                    sb.Append($"{line.Key}: {line.Value}");
                }
            }
            return sb.ToString();
        }
    }
}
