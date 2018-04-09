using FastSQL.Core;
using System;

namespace FastSQL.Sync.Core
{
    public interface IPuller : IVendorVerifier, IOptionManager
    {
        void PullAll();
        void PullRecent();
    }

    public interface IEntityPuller: IPuller
    {
        IEntityPuller SetEntity(Guid entityId);
    }

    public interface IAttributePuller: IPuller
    {
        IAttributePuller SetAttribute(Guid attributeId);
    }
}
