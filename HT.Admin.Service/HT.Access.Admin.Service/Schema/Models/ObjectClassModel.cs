using System.Collections.Generic;

namespace HT.Access.Admin.Service.Schema.Models
{
    internal class ObjectClassModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsObsolete { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsStructural { get; set; }
        public bool IsAuxiliary { get; set; }
        public string ParentObjectClassName { get; set; }
        public List<Attribute> Attributes { get; set; }

        internal class Attribute
        {
            public string Name { get; set; }
            public bool IsRequired { get; set; }
        }
    }
}
