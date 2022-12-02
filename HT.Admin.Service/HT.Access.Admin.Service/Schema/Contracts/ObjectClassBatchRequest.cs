using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HT.Access.Admin.Service.Schema.Models;

namespace HT.Access.Admin.Service.Schema.Contracts
{
    public class ObjectClassBatchRequest
    {
        public CommandOptions Options { get; set; }
        public List<ObjectClassOperationRequest> Operations { get; set; }
    }
}
