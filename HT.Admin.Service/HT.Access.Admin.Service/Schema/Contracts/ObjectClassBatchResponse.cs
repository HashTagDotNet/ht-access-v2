using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HT.Access.Admin.Service.Schema.Contracts
{
    public class ObjectClassBatchResponse
    {
        public bool IsOk => TotalOperations == TotalSuccess + TotalWarnings;

        public int TotalOperations { get; set; }
        public int TotalSuccess { get; set; }
        public int TotalWarnings { get; set; }
        public int TotalErrors { get; set; }
        public List<ObjectClassOperationResponse> OperationResults { get; set; }
    }
}
