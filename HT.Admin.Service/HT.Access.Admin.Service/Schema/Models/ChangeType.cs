using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HT.Access.Admin.Service.Schema.Models
{
    public enum ChangeType
    {
        Unknown = 0,
        Add = 1, 
        Modify = 2,
        Delete = 3,
        ModRdn = 4,
        ModDn = 5
    }
}
