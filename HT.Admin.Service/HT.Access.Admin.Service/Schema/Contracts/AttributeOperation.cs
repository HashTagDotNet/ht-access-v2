using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HT.Access.Admin.Service.Schema.Models;

namespace HT.Access.Admin.Service.Schema.Contracts
{
    public class AttributeOperation
    {
        public ChangeType ChangeType { get; set; }
        public string Attribute { get; set; }
        public string Value { get; set; }
        public string Value2 { get; set; }
    }
}