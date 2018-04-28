using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;

namespace FastSQL.Sync.Core
{
    public interface IIndexer : IOptionManager
    {
        void Persist(IEnumerable<object> data = null, bool lastPage = false);
        bool Is(EntityType entityType);
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
