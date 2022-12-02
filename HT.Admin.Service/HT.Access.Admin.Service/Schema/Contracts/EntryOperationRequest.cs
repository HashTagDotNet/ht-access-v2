using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HT.Access.Admin.Service.Schema.Models;

namespace HT.Access.Admin.Service.Schema.Contracts
{
    public class EntryOperationRequest
    {
        public ChangeType ChangeType { get; set; }
        public string ClientReference { get; set; }
        public CommandOptions Options { get; set; } = new CommandOptions() { IgnoreIfExists = true };
        public int Ordinal { get; set; }
        public string Dn { get; set; }
        public List<string> ObjectClasses { get; set; }
        public List<AttributeOperation> Operations { get; set; }
    }
}