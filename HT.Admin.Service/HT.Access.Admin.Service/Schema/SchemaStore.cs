using System.Threading;
using System.Threading.Tasks;
using HT.Access.Admin.Service.Schema.Interfaces;
using HT.Access.Admin.Service.Schema.Models;

namespace HT.Access.Admin.Service.Schema
{
    internal class SchemaStore : ISchemaStore
    {
        private readonly ISchemaPersister _persister;

        public SchemaStore(ISchemaPersister persister)
        {
            _persister = persister;
        }

        public async Task<AttributeModel> GetAttributeByName(string attributeName, CancellationToken cancellationToken = default)
        {
            return await _persister.GetAttributeByName(attributeName, cancellationToken).ConfigureAwait(false);
        }

        public async Task InsertAttribute(AttributeModel attribute, CancellationToken cancellationToken = default)
        {
            await _persister.InsertAttribute(attribute, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> IsAttributeUsed(string attributeName, CancellationToken cancellationToken = default)
        {
            return await _persister.IsAttributeUsed(attributeName, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAttributeByName(string attributeName, CancellationToken cancellationToken = default)
        {
            await _persister.DeleteAttributeByName(attributeName, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAttribute(AttributeModel attribute, CancellationToken cancellationToken = default)
        {
            await _persister.UpdateAttribute(attribute, cancellationToken).ConfigureAwait(false);
        }
    }
}
