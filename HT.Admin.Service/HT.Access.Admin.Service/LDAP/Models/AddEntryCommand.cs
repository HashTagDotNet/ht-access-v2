using HT.Access.Admin.Service.Cryptography.Interfaces;

namespace HT.Access.Admin.Service.LDAP.Models
{
    public class AddEntryCommand : LdifCommandBase
    {
        public AddEntryCommand(DistinguishedName dn, ICryptographyService cryptoService, params string[] objectClasses) : base(dn, DnChangeType.Add, cryptoService)
        {
            if (objectClasses == null || objectClasses.Length == 0)
            {
                base.AddAttribute(OBJECT_CLASS, OBJECT_CLASS_TOP);
            }
            else
            {
                base.AddAttribute(OBJECT_CLASS, objectClasses);
            }

            AddAttribute(dn.Rdn.Attribute, dn.Rdn.Value);
        }
    }
}
