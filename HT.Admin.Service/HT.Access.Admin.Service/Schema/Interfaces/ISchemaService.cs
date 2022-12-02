using System.Threading.Tasks;
using HT.Access.Admin.Service.Schema.Contracts;

namespace HT.Access.Admin.Service.Schema.Interfaces
{
    public interface ISchemaService
    {
        Task<AttributeBatchResponse> ExecuteAttributeBatchAsync(AttributeBatchRequest batchRequest);
        Task<ObjectClassBatchResponse> ExecuteObjectClassBatchAsync(ObjectClassBatchRequest batchRequest);
        Task<EntryBatchResponse> ExecuteEntryBatchAsync(EntryBatchRequest batchRequest);
    }
}