using HT.Access.Admin.Service.Schema.Contracts;
using HT.Access.Admin.Service.Schema.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HT.Access.Api.Controllers.Schema
{
    [Route("api/schema/[controller]")]
    [ApiController]
    public class AttributesController : ControllerBase
    {
        private readonly ISchemaService _svc
            ;

        public AttributesController(ISchemaService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        public async Task<IActionResult> RunBatch(AttributeBatchRequest request)
        {
            var response = await _svc.ExecuteAttributeBatch(request).ConfigureAwait(false);
            return new OkObjectResult(response);
        }
    }
}
