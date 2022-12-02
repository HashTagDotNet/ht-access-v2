using System.Collections.Generic;
using HT.Access.Admin.Service.LDAP.Models;
using HT.Access.Admin.Service.Schema.Models;

namespace HT.Access.Admin.Service.Schema.Contracts
{
    public class AttributeBatchRequest
    {
        public CommandOptions Options { get; set; }
        public List<AttributeOperationRequest> Operations { get; set; }
    }

 

   

}
