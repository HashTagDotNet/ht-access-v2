using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using HT.Access.Admin.Service.LDAP.Models;
using HT.Access.Admin.Service.Schema.Contracts;
using HT.Access.Admin.Service.Schema.Interfaces;
using HT.Access.Admin.Service.Schema.Models;
using HT.Common.Collections;

namespace HT.Access.Admin.Service.Schema
{
    internal class SchemaService : ISchemaService
    {
        private readonly ISchemaStore _store;

        public SchemaService(ISchemaStore schemaStore)
        {
            _store = schemaStore;
        }

        public async Task<AttributeBatchResponse> ExecuteAttributeBatchAsync(AttributeBatchRequest batchRequest)
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
                        batchResponse.TotalOperations++;
                        continue;
                    }

                    request.Ordinal = batchIndex;
                    var executedResponse =
                        await executeAttributeOperation(request, batchResponse).ConfigureAwait(false);
                }
            }

            return batchResponse;
        }

        public async Task<ObjectClassBatchResponse> ExecuteObjectClassBatchAsync(ObjectClassBatchRequest batchRequest)
        {
            var batchResponse = new ObjectClassBatchResponse()
            {
                TotalOperations = batchRequest?.Operations?.Count ?? -1
            };
            if (batchRequest?.Operations is { Count: > 0 })
            {
                for (int batchIndex = 0; batchIndex < batchRequest.Operations.Count; batchIndex++)
                {
                    ObjectClassOperationRequest request = batchRequest.Operations[batchIndex];
                    if (request == null)
                    {
                        batchResponse.TotalOperations++;
                        continue;
                    }

                    request.Ordinal = batchIndex;
                    var executedResponse =
                        await executeObjectClassOperation(request, batchResponse).ConfigureAwait(false);
                }
            }

            return batchResponse;
        }

        /// <inheritdoc />
        public async Task<EntryBatchResponse> ExecuteEntryBatchAsync(EntryBatchRequest batchRequest)
        {
            var batchResponse = new EntryBatchResponse()
            {
                TotalOperations = batchRequest?.Operations?.Count ?? -1
            };
            if (batchRequest?.Operations is { Count: > 0 })
            {
                for (int batchIndex = 0; batchIndex < batchRequest.Operations.Count; batchIndex++)
                {
                    EntryOperationRequest request = batchRequest.Operations[batchIndex];
                    if (request == null)
                    {
                        batchResponse.TotalOperations++;
                        continue;
                    }

                    request.Ordinal = batchIndex;
                    EntryOperationResponse executedResponse =
                        await executeEntryOperationAsync(request, batchResponse).ConfigureAwait(false);
                }
            }

            return batchResponse;
        }

        private async Task<EntryOperationResponse> executeEntryOperationAsync(EntryOperationRequest request,
            EntryBatchResponse batchResponse)
        {
            var operationResponse = new EntryOperationResponse
            {
                ClientReference = request.ClientReference,
                Ordinal = request.Ordinal,
            };
            switch (request.ChangeType)
            {
                case ChangeType.Add:
                    await addEntryAsync(request, operationResponse, batchResponse).ConfigureAwait(false);
                    break;
                case ChangeType.Modify:
                    await modifyEntryAsync(request, operationResponse, batchResponse).ConfigureAwait(false);
                    break;
                case ChangeType.Delete:
                    await deleteEntryAsync(request, operationResponse, batchResponse).ConfigureAwait(false);
                    break;
                case ChangeType.Unknown:
                default:
                    operationResponse.OperationStatus = LdifStatusCode.Other;
                    break;
            }

            batchResponse.OperationResults ??= new();
            batchResponse.OperationResults.Add(operationResponse);
            if (operationResponse.OperationStatus != LdifStatusCode.Success)
            {
                batchResponse.TotalErrors++;
            }
            else
            {
                batchResponse.TotalSuccess++;
            }

            return operationResponse;
        }

        private async Task deleteEntryAsync(EntryOperationRequest request, EntryOperationResponse operationResponse,
            EntryBatchResponse batchResponse)
        {
            throw new NotImplementedException();
        }

        private async Task modifyEntryAsync(EntryOperationRequest request, EntryOperationResponse operationResponse,
            EntryBatchResponse batchResponse)
        {
            throw new NotImplementedException();
        }

        private async Task addEntryAsync(EntryOperationRequest request, EntryOperationResponse operationResponse,
            EntryBatchResponse batchResponse)
        {
            throw new NotImplementedException();
        }

        private async Task<ObjectClassOperationResponse> executeObjectClassOperation(
            [DisallowNull] ObjectClassOperationRequest request, [DisallowNull] ObjectClassBatchResponse batchResponse)
        {
            var operationResponse = new ObjectClassOperationResponse
            {
                ClientReference = request.ClientReference,
                Ordinal = request.Ordinal,
            };
            switch (request.ChangeType)
            {
                case ChangeType.Add:
                    await addObjectClass(request, operationResponse, batchResponse).ConfigureAwait(false);
                    break;
                case ChangeType.Modify:
                    await modifyObjectClass(request, operationResponse, batchResponse).ConfigureAwait(false);
                    break;
                case ChangeType.Delete:
                    await deleteObjectClass(request, operationResponse, batchResponse).ConfigureAwait(false);
                    break;
                case ChangeType.Unknown:
                default:
                    operationResponse.OperationStatus = LdifStatusCode.Other;
                    break;
            }

            batchResponse.OperationResults ??= new();
            batchResponse.OperationResults.Add(operationResponse);
            if (operationResponse.OperationStatus != LdifStatusCode.Success)
            {
                batchResponse.TotalErrors++;
            }
            else
            {
                batchResponse.TotalSuccess++;
            }

            return operationResponse;
        }

        private async Task deleteObjectClass(ObjectClassOperationRequest request,
            ObjectClassOperationResponse operationResponse, ObjectClassBatchResponse batchResponse)
        {
            throw new NotImplementedException();
        }

        private async Task modifyObjectClass(ObjectClassOperationRequest request,
            ObjectClassOperationResponse operationResponse, ObjectClassBatchResponse batchResponse)
        {
            throw new NotImplementedException();
        }

        private async Task addObjectClass(ObjectClassOperationRequest request,
            ObjectClassOperationResponse operationResponse, ObjectClassBatchResponse batchResponse)
        {
            bool objectExists = await _store.DoesObjectClassExist(request.Name).ConfigureAwait(false);
            if (objectExists)
            {
                operationResponse.OperationStatus = request.Options.IgnoreIfExists
                    ? LdifStatusCode.Success
                    : LdifStatusCode.AttributeOrValueExists;
                return;
            }

            List<Task<bool>> allTasks = new();
            if (request.MayAttributes is { Count: > 0 })
            {
                foreach (var attr in request.MayAttributes)
                {
                    allTasks.Add(_store.DoesAttributeExist(attr));
                }
            }

            if (request.MustAttributes is { Count: > 0 })
            {
                foreach (var attr in request.MustAttributes)
                {
                    allTasks.Add(_store.DoesAttributeExist(attr));
                }
            }

            if (allTasks.Count > 0)
            {
                var existenceResults = await Task.WhenAll(allTasks).ConfigureAwait(false);

                if (existenceResults.FindIndex(a => a == false) > -1)
                {
                    operationResponse.OperationStatus = LdifStatusCode.NoSuchAttribute;
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.ParentClass))
            {
                bool parentExists = await _store.DoesObjectClassExist(request.ParentClass).ConfigureAwait(false);
                if (!parentExists)
                {
                    operationResponse.OperationStatus = LdifStatusCode.NoSuchObject;
                    return;
                }
            }

            ObjectClassModel model = new()
            {
                Description = request.Description,
                IsAbstract = request.IsAbstract,
                IsAuxiliary = request.IsAuxiliary,
                IsObsolete = request.IsObsolete,
                IsStructural = request.IsStructural,
                Name = request.Name,
                ParentObjectClassName = request.ParentClass
            };

            if (request?.MayAttributes is { Count: > 0 })
            {
                model.Attributes ??= new List<ObjectClassModel.Attribute>();

                foreach (var attr in request.MayAttributes)
                {
                    model.Attributes.Add(new() { IsRequired = false, Name = attr });
                }
            }

            if (request?.MustAttributes is { Count: > 0 })
            {
                model.Attributes ??= new List<ObjectClassModel.Attribute>();
                foreach (var attr in request.MustAttributes)
                {
                    model.Attributes.Add(new() { IsRequired = true, Name = attr });
                }
            }

            await _store.InsertObjectClass(model).ConfigureAwait(false);
        }

        private async Task<AttributeOperationResponse> executeAttributeOperation(AttributeOperationRequest request,
            [DisallowNull] AttributeBatchResponse batchResponse)
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

        private async Task deleteAtribute(AttributeOperationRequest request,
            AttributeOperationResponse operationResponse, AttributeBatchResponse batchResponse)
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

        private async Task modifyAtribute(AttributeOperationRequest request,
            AttributeOperationResponse operationResponse, AttributeBatchResponse batchResponse)
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
                IsSystemEntry = request.IsInternalAttribute,
                AllowMultipleValues = request.AllowMultipleValues,
                Name = request.Name,
                Obsolete = request.IsObsolete,
                Type = request.Type
            }).ConfigureAwait(false);

            operationResponse.OperationStatus = LdifStatusCode.Success;
        }

        private async Task addAtribute(AttributeOperationRequest request, AttributeOperationResponse operationResponse,
            AttributeBatchResponse batchResponse)
        {
            try
            {
                AttributeModel existingAttribute = await _store.GetAttributeByName(request.Name);
                if (existingAttribute != null)
                {
                    if (request.Options is { IgnoreIfExists: true })
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
                    IsSystemEntry = request.IsInternalAttribute,
                    AllowMultipleValues = request.AllowMultipleValues,
                    Name = request.Name,
                    Obsolete = request.IsObsolete,
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