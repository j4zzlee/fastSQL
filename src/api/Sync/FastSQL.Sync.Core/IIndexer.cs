using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;

namespace FastSQL.Sync.Core
{
    public interface IIndexer : IOptionManager
    {
        void Persist(IEnumerable<object> data = null);
        void OnReport(Action<string> reporter);
        void Report(string message);
        void StartIndexing(bool cleanAll);
        void EndIndexing();
        void BeginTransaction();
        void Commit();
        void RollBack();
    }

    public interface IEntityIndexer: IIndexer
    {
        IEntityIndexer SetEntity(EntityModel entity);
        bool IsImplemented(string processorId, string providerId);
    }

    public interface IAttributeIndexer: IIndexer
    {
        IAttributeIndexer SetAttribute(AttributeModel attribute, EntityModel entity);
        bool IsImplemented(string attributeProcessorId, string entityProcessorId, string providerId);
    }
}
