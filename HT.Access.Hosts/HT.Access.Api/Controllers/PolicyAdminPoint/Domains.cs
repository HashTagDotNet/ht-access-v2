using HT.Access.Admin.Service.AccessControl.Interfaces;
using HT.Access.Admin.Service.AccessControl.Models;
using Microsoft.AspNetCore.Mvc;

namespace HT.Access.Api.Controllers.PolicyAdminPoint
{
    [Route("api/pap/[controller]")]
    [ApiController]
    public class Domains : ControllerBase
    {
        private readonly IDomainService _svc;

        public Domains(IDomainService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDomainAsync(Domain domain)
        {
            await _svc.AddDomain(new Domain()
            {
                Code = "mydomain"
            }).ConfigureAwait(false);
            return new OkResult();
        }
    }
}
