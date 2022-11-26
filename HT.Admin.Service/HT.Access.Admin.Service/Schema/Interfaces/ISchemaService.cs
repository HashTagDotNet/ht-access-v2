using System.Threading.Tasks;
using HT.Access.Admin.Service.Schema.Contracts;
using HT.Common.ApiMessaging;

namespace HT.Access.Admin.Service.Schema.Interfaces
{
    public interface ISchemaService
    {
        Task<AttributeBatchResponse> ExecuteAttributeBatch(AttributeBatchRequest batchRequest);
        

    }
}
