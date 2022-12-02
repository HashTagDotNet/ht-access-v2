using System.Collections.Generic;
using HT.Access.Admin.Service.Schema.Models;

namespace HT.Access.Admin.Service.Schema.Contracts
{
    public class EntryBatchRequest
    {
        public CommandOptions Options { get; set; }
        public List<EntryOperationRequest> Operations { get; set; }
    }
}