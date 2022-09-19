using HT.Access.Admin.Service.Cryptography;
using HT.Access.Admin.Service.LDAP;
using Xunit;
namespace HT.Access.Admin.Service.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var builder = new LdifBuilder(new CryptographyService());
            var cmd = builder.AddEntry("do=descartes", "domain", "top");
            builder.AddEntry("ap=vNext,do=descartes", "app", "top");
            var ldifString = builder.Build();
        }
    }
}