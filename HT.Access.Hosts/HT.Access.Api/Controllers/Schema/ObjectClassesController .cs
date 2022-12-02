using HT.Access.Admin.Service.Schema.Contracts;
using HT.Access.Admin.Service.Schema.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HT.Access.Api.Controllers.Schema
{
    [Route("api/schema/[controller]")]
    [ApiController]
    public class ObjectClassesController : ControllerBase
    {
        private readonly ISchemaService _svc;

        public ObjectClassesController(ISchemaService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        public async Task<IActionResult> RunBatch(ObjectClassBatchRequest request)
        {
            var response = await _svc.ExecuteObjectClassBatchAsync(request).ConfigureAwait(false);
            return new OkObjectResult(response);
        }
    }
}