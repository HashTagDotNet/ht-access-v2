namespace HT.Access.Admin.Service.Schema.Models
{
    public class AttributeModel
    {
        public AttributeType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Obsolete { get; set; }
        public bool AllowMultipleValues { get; set; }
        public bool IsSystemEntry { get; set; }
    }
}
