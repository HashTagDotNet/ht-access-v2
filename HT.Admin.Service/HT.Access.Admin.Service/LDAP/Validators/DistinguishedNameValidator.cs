using FluentValidation;
using HT.Access.Admin.Service.LDAP.Models;

namespace HT.Access.Admin.Service.Models.Validators
{
    public class DistinguishedNameValidator:AbstractValidator<DistinguishedName>
    {
        public DistinguishedNameValidator()
        {
            //RuleFor(dn => dn).NotNull();
            //RuleFor(dn => dn.ToString()).Length(1, 1700);
            //RuleForEach(dn => dn.RdnList).SetValidator(new RelativeDistinguishedNameValidator());
            
            //RuleFor(dn => dn.RootRdn.Attribute).NotNull();
            //RuleFor(dn => dn.RootRdn.Attribute).NotEmpty();
            //RuleFor(dn => dn.RootRdn.Attribute).Must(attr =>
            //        string.Compare(attr, TypeCodes.Domain, StringComparison.InvariantCultureIgnoreCase) == 0)
            //    .WithMessage($"Root RDN must be {TypeCodes.Domain}=");
            

        }
    }
}
