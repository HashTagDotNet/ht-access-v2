using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HT.Access.Admin.Service.LDAP.Models
{
    public enum DnChangeType
    {
        Add,
        Delete,
        Modify,
        ModifyRdn,
        ModifyDn,
    }
}
