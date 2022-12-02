using System.Collections.Generic;
using HT.Access.Admin.Service.Schema.Models;

namespace HT.Access.Admin.Service.Schema.Contracts
{
    public class ObjectClassOperationRequest
    {
        public ChangeType ChangeType { get; set; }
        public string ClientReference { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsObsolete { get; set; }
        public CommandOptions Options { get; set; } = new CommandOptions() { IgnoreIfExists = true };
        public int Ordinal { get; set; }
        public string ParentClass { get; set; }
        public List<string> MayAttributes { get; set; }
        public List<string> MustAttributes { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsStructural { get; set; }
        public bool IsAuxiliary { get; set; }
    }
}