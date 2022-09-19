using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HT.Access.Admin.Service.LDAP.Models;

namespace HT.Access.Admin.Service.LDAP.Interfaces
{
    /// <summary>
    /// https://docs.oracle.com/cd/B14099_19/idmanage.1012/b15883/ldif_appendix002.htm
    /// </summary>
    public interface ILdifBuilder
    {
        /// <summary>
        /// <inheritdoc cref="AddEntry(DistinguishedName,string[])"/>.
        /// </summary>
        AddEntryCommand AddEntry(string dn, params string[] objectClasses);

        /// <summary>
        /// Starts an 'changetype: add' Ldif Stanza for adding a new entry into the Directory (DIT)
        /// </summary>
        /// <param name="dn">Fully qualified <see cref="DistinguishedName"/> (required) </param>
        /// <param name="objectClasses">List of object classes for this entry. If not <paramref name="objectClasses"/>  are provided, 'top' is automatically added</param>
        /// <returns></returns>
        AddEntryCommand AddEntry(DistinguishedName dn, params string[] objectClasses);

        List<LdifCommandBase> Commands { get; }
    }
}
