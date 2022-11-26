using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HT.Access.Admin.Service.AccessControl.Interfaces;
using HT.Access.Admin.Service.AccessControl.Models;

namespace HT.Access.Admin.Service.AccessControl
{
    public class DomainService:IDomainService
    {
        private readonly IAdminPersister _persister;

        public DomainService(IAdminPersister persister)
        {
            _persister = persister;
        }
        public async Task AddDomain(Domain domain)
        {
            var existingDomain = await GetDomain("do=mydomain");
            if (existingDomain != null)
            {
                throw new InvalidOperationException("domain exists");
            }
         
            await _persister.AddDomain(domain).ConfigureAwait(false);
        }

        public async Task UpdateDomain(Domain domain)
        {
            throw new NotImplementedException();
        }

        public async Task RenameDomain(Domain domain)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteDomain(Domain domain)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Domain>> GetDomains()
        {
            throw new NotImplementedException();
        }

        public async Task<Domain> GetDomain(string domainDn)
        {
            return await Task.FromResult((Domain)null);
        }

        public async Task<IList<Application>> GetApplicationsForDomain(string domainDn)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<OrgUnit>> GetOrgUnitsForDomain(string domainDn)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Actor>> GetActorsForDomain(string domainDn)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<DomainGroup>> GetDomainGroupsForDomain(string domainDn)
        {
            throw new NotImplementedException();
        }
    }
}
