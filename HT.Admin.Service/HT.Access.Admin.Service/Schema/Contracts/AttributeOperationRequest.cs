using HT.Access.Admin.Service.Schema.Models;

namespace HT.Access.Admin.Service.Schema.Contracts
{
    public class AttributeOperationRequest
    {
        public ChangeType ChangeType { get; set; }
        public string ClientReference { get; set; }
        public AttributeType Type { get; set; } = AttributeType.String;
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Obsolete { get; set; }
        public bool AllowMultipleValues { get; set; } = true;
        public bool IsSystemEntry { get; set; } = false;
        public CommandOptions Options { get; set; } = new CommandOptions() { IgnoreIfExists = true };
        public int Ordinal { get; set; }
    }
}
