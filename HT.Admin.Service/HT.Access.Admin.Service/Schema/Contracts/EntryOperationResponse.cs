using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HT.Access.Admin.Service.LDAP.Models;

namespace HT.Access.Admin.Service.Schema.Contracts
{
    public class EntryOperationResponse
    {
        public int Ordinal { get; set; }
        public string ClientReference { get; set; }
        public LdifStatusCode OperationStatus { get; set; }
    }
}