using System.Threading.Tasks;
using HT.Access.Admin.Service.AccessControl.Models;
using HT.Access.Admin.Service.LDAP.Models;

namespace HT.Access.Admin.Service.AccessControl.Interfaces
{
    public interface IAdminPersister
    {
        Task AddDomain(Domain domain);
    }
}
