﻿using HT.Access.Admin.Service.LDAP.Models;

namespace HT.Access.Admin.Service.Schema.Contracts
{
    public class AttributeOperationResponse
    {
        public int Ordinal { get; set; }
        public string ClientReference { get; set; }
        public LdifStatusCode OperationStatus { get; set; }
    }
}
