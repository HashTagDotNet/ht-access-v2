using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HT.Access.Admin.Service.LDAP.Models
{
    /// <summary>
    /// https://docs.ldap.com/specs/rfc4511.txt
    /// <para>https://ldap.com/ldap-result-code-reference-core-ldapv3-result-codes/#rc-noSuchObject</para>
    /// </summary>
    public enum LdifStatusCode
    {
        /// <summary>
        /// Applicable operation types: add, bind, delete, extended, modify, modify DN, search. The success result code is used to indicate that the associated operation completed successfully.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The noSuchAttribute result code indicates that the request targeted an attribute that does not exist in the specified entry.
        /// Indicates that the request attempted to delete one or more attribute values that don’t exist in the targeted entry,
        /// that the request attempted to delete an entire attribute that does not have any values in the targeted entry, or that
        /// attempted to increment the value of an attribute that does not have any values in the targeted entry
        /// </summary>
        NoSuchAttribute = 16,

        /// <summary>
        /// The undefinedAttributeType result code indicates that the request attempted to provide one or more values for an attribute type
        /// that is not defined in the server schema.
        ///<para>For an add request, it indicates that the provided entry included an attribute for which there is no
        /// corresponding attribute type definition in the schema.</para>
        /// <para>For a modify request, it indicates that a modification attempted to add one or more values, or to
        /// replace the entire set of values, for an attribute type that is not defined in the server schema.</para>
        /// </summary>
        UndefinedAttributeType = 17,

        /// <summary>
        /// The attributeOrValueExists result code indicates that the requested operation would have resulted in an attribute
        /// in which the same value appeared more than once.
        /// <para>For an add request, it indicates that at least one of the attributes in the provided entry had a duplicate value.</para>
        /// <para>For a modify request, it indicates that either an add or replace modification included the same value multiple times,
        /// or that an add modification attempted to add a value that already exists in the entry.</para>
        /// </summary>
        AttributeOrValueExists = 20,

        /// <summary>
        /// The noSuchObject result code indicates that the requested operation targeted an entry that does not exist within the DIT.
        /// <para>For an add request, it means that the immediate parent of the entry to be added does not exist and that the
        /// DN of the entry to be added does not match any of the configured naming contexts.</para>
        /// <para>For a compare, delete, or modify request, it indicates that the targeted entry does not exist.</para>
        /// <para>For a modify DN request, it indicates that either the targeted entry does not exist, or that the
        /// provided new superior DN references an entry that does not exist.</para>
        /// </summary>
        NoSuchObject = 32,

        /// <summary>
        /// The namingViolation result code indicates that the requested add or modify DN operation would have resulted in an entry that
        /// violates some naming constraint within the server. Some of the potential
        /// </summary>
        NamingViolation = 64,

        /// <summary>
        /// Applicable operation types: add, modify, modify DN.
        /// <para>The objectClassViolation result code indicates that the requested operation would have resulted in an entry that has an inappropriate set of object classes, or whose attributes violate the constraints associated with its set of object classes. Some of the possible reasons for this include:</para>
        /// <para>
        /// The entry would have included an object class that is not defined in the schema.<br />
        /// The entry would not have included any structural object class.<br />
        /// The entry would have included multiple structural object classes.<br />
        /// The entry would have included an auxiliary object class that is not permitted to be used in conjunction with its structural object class.<br />
        /// The entry would have included an abstract object class that is not a superclass for any of the structural or auxiliary object classes for that entry.<br />
        /// The entry would have been missing an attribute that is required by one of its object classes or its DIT content rule.<br />
        /// The entry would have included an object class that is not permitted by any of its object classes, or that is prohibited by its DIT content rule.<br />
        /// </para>
        /// </summary>
        ObjectClassViolation = 65,

        /// <summary>
        /// Applicable operation types: add, modify DN
        /// The entryAlreadyExists result code indicates that the requested operation would have resulted in an entry
        /// with the same DN as an entry that already exists in the server.
        /// For an add request, it means that the server already contains an entry whose
        /// DN matches the DN contained in the request.
        /// </summary>
        EntryAlreadyExists =68,

        /// <summary>
        /// The other result code is used when a problem occurs for which none of the other result codes is more appropriate.
        /// It is the correct result code to use in the event that an internal error occurs within the server (although some servers mistakenly
        /// use operationsError (1) for this purpose), but the other result code may be used for additional kinds of problems as well.
        /// </summary>
        Other = 80
    }
}
