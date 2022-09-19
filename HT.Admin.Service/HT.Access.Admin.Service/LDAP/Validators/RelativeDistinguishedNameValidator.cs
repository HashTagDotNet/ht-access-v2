//using System.Text.RegularExpressions;

//namespace HT.Access.Admin.Service.Models.Validators
//{
//    public class RelativeDistinguishedNameValidator:AbstractValidator<DistinguishedName.RelativeDistinguishedName>
//    {
//        private static Regex __invalidCharacters = new Regex("[=,]", RegexOptions.Compiled);

//        public RelativeDistinguishedNameValidator()
//        {
//            RuleFor(rdn => rdn.Attribute).NotEmpty();
//            RuleFor(rdn => rdn.Attribute).Length(1,10);
//            RuleFor(rdn => rdn.Value).NotEmpty().WithErrorCode("ERR-NOVAL");
//            RuleFor(rdn => rdn.Value).Length(1, 50).WithErrorCode("ERR-VAL-LEN");

//            RuleFor(rdn => rdn.Value)
//                .Must(validateAttributeHasValidCharacters)
//                .WithMessage("RDN values must not contain reserved characters: '=', ','");
//            RuleFor(rdn => rdn.Attribute).Must(validateAttributeIsKnownObjectType);
//        }
        
//        private bool validateAttributeIsKnownObjectType(string attributeCode) => !string.IsNullOrWhiteSpace(TypeCodes.FindCode(attributeCode));
//        private bool validateAttributeHasValidCharacters(string value) => !__invalidCharacters.IsMatch(value);
        
//    }
//}
