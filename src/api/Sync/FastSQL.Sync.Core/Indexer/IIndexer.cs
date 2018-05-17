using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;

namespace FastSQL.Sync.Core.Indexer
{
    public interface IIndexer : IOptionManager
    {
        IIndexer Persist(IEnumerable<object> data = null);
        IIndexer OnReport(Action<string> reporter);
        IIndexer StartIndexing(bool cleanAll);
        IIndexer EndIndexing();
        IIndexer BeginTransaction();
        IIndexer Commit();
        IIndexer RollBack();
        IIndexer SetIndex(IIndexModel model);
    }

    public interface IEntityIndexer: IIndexer
    {
        bool IsImplemented(string processorId, string providerId);
    }

    public interface IAttributeIndexer: IIndexer
    {
        bool IsImplemented(string attributeProcessorId, string entityProcessorId, string providerId);
    }
}
