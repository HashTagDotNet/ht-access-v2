using HT.Access.Admin.Service.LDAP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace HT.Access.Admin.Service.LDAP.Interfaces
{
    public interface ILdifRunner
    {
        Task Run(List<LdifCommandBase> commands, CancellationToken cancellationToken = default);
    }
}
