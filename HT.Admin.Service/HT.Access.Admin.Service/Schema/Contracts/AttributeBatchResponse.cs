using System.Collections.Generic;

namespace HT.Access.Admin.Service.Schema.Contracts
{
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
