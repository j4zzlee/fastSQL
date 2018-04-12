using FastSQL.Core;
using System;
using System.Collections.Generic;

namespace FastSQL.Sync.Core
{
    public interface IIndexer : IProcessorVerifier, IRichProviderVerifier, IOptionManager
    {
        void Persist(IEnumerable<object> data = null);
    }

    public interface IEntityIndexer: IIndexer
    {
        IEntityIndexer SetEntity(Guid entityId);
    }

    public interface IAttributeIndexer: IIndexer
    {
        IAttributeIndexer SetAttribute(Guid attributeId);
    }
}
