using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using HT.Access.Admin.Service.LDAP.Models;
using HT.Access.Admin.Service.Schema.Contracts;
using HT.Access.Admin.Service.Schema.Interfaces;
using HT.Access.Admin.Service.Schema.Models;

namespace HT.Access.Admin.Service.Schema
{
    internal class SchemaService : ISchemaService
    {
        private readonly ISchemaStore _store;

        public SchemaService(ISchemaStore schemaStore)
        {
            _store = schemaStore;
        }
        public async Task<AttributeBatchResponse> ExecuteAttributeBatch(AttributeBatchRequest batchRequest)
        {
            var batchResponse = new AttributeBatchResponse()
            {
                TotalOperations = batchRequest?.Operations?.Count ?? -1
            };

            if (batchRequest?.Operations is { Count: > 0 })
            {

                for (int batchIndex = 0; batchIndex < batchRequest.Operations.Count; batchIndex++)
                {
                    AttributeOperationRequest request = batchRequest.Operations[batchIndex];
                    if (request == null)
                    {
                        continue;
                    }
                    request.Ordinal = batchIndex;
                    var executedResponse = await executeAttributeOperation(request, batchResponse).ConfigureAwait(false);

                }
            }
            return batchResponse;
        }

        private async Task<AttributeOperationResponse> executeAttributeOperation(AttributeOperationRequest request, [DisallowNull] AttributeBatchResponse batchResponse)
        {
            var operationResponse = new AttributeOperationResponse
            {
                ClientReference = request.ClientReference,
                Ordinal = request.Ordinal,
            };
            switch (request.ChangeType)
            {
                case ChangeType.Add:
                    await addAtribute(request, operationResponse, batchResponse).ConfigureAwait(false);
                    break;
                case ChangeType.Modify:
                    await modifyAtribute(request, operationResponse, batchResponse).ConfigureAwait(false);
                    break;
                case ChangeType.Delete:
                    await deleteAtribute(request, operationResponse, batchResponse).ConfigureAwait(false);
                    break;
                case ChangeType.Unknown:
                default:
                    operationResponse.OperationStatus = LdifStatusCode.Other;
                    break;
            }

            batchResponse.OperationResults ??= new();
            batchResponse.OperationResults.Add(operationResponse);
            return operationResponse;
        }

        private async Task deleteAtribute(AttributeOperationRequest request, AttributeOperationResponse operationResponse, AttributeBatchResponse batchResponse)
        {
            AttributeModel existingAttribute = await _store.GetAttributeByName(request.Name);
            if (existingAttribute == null)
            {
                if (request.Options is { IgnoreIfExists: true })
                {
                    operationResponse.OperationStatus = LdifStatusCode.Success;
                    batchResponse.TotalSuccess++;
                }
                else
                {
                    operationResponse.OperationStatus = LdifStatusCode.NoSuchAttribute;
                    batchResponse.TotalErrors++;
                }

                return;
            }

            bool isAttributeUsed = await _store.IsAttributeUsed(request.Name).ConfigureAwait(false);
            if (isAttributeUsed)
            {
                operationResponse.OperationStatus = LdifStatusCode.AttributeOrValueExists;
                batchResponse.TotalErrors++;
                return;
            }

            await _store.DeleteAttributeByName(request.Name).ConfigureAwait(false);

        }

        private async Task modifyAtribute(AttributeOperationRequest request, AttributeOperationResponse operationResponse, AttributeBatchResponse batchResponse)
        {
            AttributeModel existingAttribute = await _store.GetAttributeByName(request.Name);
            if (existingAttribute == null)
            {
                if (request.Options is { ContinueOnError: true })
                {
                    operationResponse.OperationStatus = LdifStatusCode.Success;
                    batchResponse.TotalSuccess++;
                }
                else
                {
                    operationResponse.OperationStatus = LdifStatusCode.NoSuchAttribute;
                    batchResponse.TotalErrors++;
                }

                return;
            }

            //TODO need to verify if 'allowMultiple' moves from true to false that no entries have multiple values; value type changes that data can be converted into new type
            await _store.UpdateAttribute(new AttributeModel()
            {
                Description = request.Description,
                IsSystemEntry = request.IsSystemEntry,
                AllowMultipleValues = request.AllowMultipleValues,
                Name = request.Name,
                Obsolete = request.Obsolete,
                Type = request.Type
            }).ConfigureAwait(false);

            operationResponse.OperationStatus = LdifStatusCode.Success;

        }

        private async Task addAtribute(AttributeOperationRequest request, AttributeOperationResponse operationResponse, AttributeBatchResponse batchResponse)
        {
            try
            {
                AttributeModel existingAttribute = await _store.GetAttributeByName(request.Name);
                if (existingAttribute != null)
                {
                    if (request.Options is {IgnoreIfExists:true})
                    {
                        operationResponse.OperationStatus = LdifStatusCode.Success;
                        batchResponse.TotalSuccess++;
                    }
                    else
                    {
                        operationResponse.OperationStatus = LdifStatusCode.EntryAlreadyExists;
                        batchResponse.TotalErrors++;
                    }
                    return;
                }

                await _store.InsertAttribute(new AttributeModel()
                {
                    Description = request.Description,
                    IsSystemEntry = request.IsSystemEntry,
                    AllowMultipleValues = request.AllowMultipleValues,
                    Name = request.Name,
                    Obsolete = request.Obsolete,
                    Type = request.Type
                }).ConfigureAwait(false);

                operationResponse.OperationStatus = LdifStatusCode.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
