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

    public class AttributeOperationResponse
    {
        public int Ordinal { get; set; }
        public string ClientReference { get; set; }
        public LdifStatusCode OperationStatus { get; set; }
    }

    public class AttributeBatchResponse
    {
        public bool IsOk => TotalOperations == TotalSuccess + TotalWarnings;

        public int TotalOperations { get; set; }
        public int TotalSuccess { get; set; }
        public int TotalWarnings { get; set; }
        public int TotalErrors { get; set; }
        public List<AttributeOperationResponse> OperationResults { get; set; }

    }

}
