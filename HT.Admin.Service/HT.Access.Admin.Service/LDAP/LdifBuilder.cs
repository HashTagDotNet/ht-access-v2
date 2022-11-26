using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HT.Access.Admin.Service.Cryptography.Interfaces;
using HT.Access.Admin.Service.LDAP.Interfaces;
using HT.Access.Admin.Service.LDAP.Models;

namespace HT.Access.Admin.Service.LDAP
{

    /// <summary>
    /// <inheritdoc cref="ILdifBuilder"/>
    /// </summary>
    public class LdifBuilder:ILdifBuilder
    {
        private readonly List<LdifCommandBase> _commands = new();
        private readonly ICryptographyService _cryptoService;

        public LdifBuilder(ICryptographyService cryptoService)
        {
            _cryptoService = cryptoService;
        }
        public AddEntryCommand AddEntry(string dn,  params string[] objectClasses)
        {
            return this.AddEntry(new DistinguishedName(dn), objectClasses);
        }
        public AddEntryCommand AddEntry(DistinguishedName dn, params string[] objectClasses)
        {
            throw new NotImplementedException();
            //var cmd = new AddEntryCommand(dn, _cryptoService, objectClasses);
            //_commands.Add(cmd);
            //return cmd;
        }

        public List<LdifCommandBase> Commands => _commands;

        public string Build()
        {
            var sb = new StringBuilder();
            foreach (var cmd in _commands)
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }

                sb.Append(cmd.Build());
            }

            return sb.ToString();
        }
    }
}
