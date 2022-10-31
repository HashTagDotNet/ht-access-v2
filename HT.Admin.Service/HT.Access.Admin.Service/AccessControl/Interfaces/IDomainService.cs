using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HT.Access.Admin.Service.AccessControl.Models;

namespace HT.Access.Admin.Service.AccessControl.Interfaces
{
    public interface IDomainService
    {
        Task AddDomain(Domain domain);
        Task UpdateDomain(Domain domain);
        Task RenameDomain(Domain domain);
        Task DeleteDomain(Domain domain);

        Task<IList<Application>> GetApplicationsForDomain(string domainDn);
        Task<IList<OrgUnit>> GetOrgUnitsForDomain(string domainDn);
        Task<IList<Actor>> GetActorsForDomain(string domainDn);
        Task<IList<DomainGroup>> GetDomainGroupsForDomain(string domainDn);



    }
}
