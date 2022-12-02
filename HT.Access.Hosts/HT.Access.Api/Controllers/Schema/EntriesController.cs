using HT.Access.Admin.Service.Schema.Contracts;
using HT.Access.Admin.Service.Schema.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HT.Access.Api.Controllers.Schema
{
    [Route("api/schema/[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly ISchemaService _svc;

        public EntriesController(ISchemaService svc)
        {
            _svc = svc;
        }


        [HttpPost]
        public async Task<IActionResult> RunBatch(EntryBatchRequest request)
        {
            var response = await _svc.ExecuteEntryBatchAsync(request).ConfigureAwait(false);
            return new OkObjectResult(response);
        }
    }
}