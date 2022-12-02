﻿using System.Threading;
using System.Threading.Tasks;
using HT.Access.Admin.Service.Schema.Models;

namespace HT.Access.Admin.Service.Schema.Interfaces
{
    internal interface ISchemaPersister
    {
        Task<AttributeModel> GetAttributeByName(string attributeName, CancellationToken cancellationToken = default);
        Task InsertAttribute(AttributeModel attribute, CancellationToken cancellationToken = default);
        Task<bool> IsAttributeUsed(string attributeName, CancellationToken cancellationToken = default);
        Task DeleteAttributeByName(string attributeName, CancellationToken cancellationToken = default);
        Task UpdateAttribute(AttributeModel attribute, CancellationToken cancellationToken = default);
        Task<bool> DoesObjectClassExist(string className, CancellationToken cancellationToken = default);
        Task<bool> DoesAttributeExist(string attributeName, CancellationToken cancellationToken = default);
        Task InsertObjectClass(ObjectClassModel model, CancellationToken cancellationToken = default);
    }
}
