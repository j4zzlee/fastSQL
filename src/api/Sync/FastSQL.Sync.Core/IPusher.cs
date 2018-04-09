using FastSQL.Core;
using System;

namespace FastSQL.Sync.Core
{
    public interface IPusher: IVendorVerifier, IOptionManager
    {
        void PushAll();
        void Push(Guid itemId);
    }

    public interface IEntityPusher: IPusher
    {
        IEntityPusher SetEntity(Guid entityId);
    }

    public interface IAttributePusher: IPusher
    {
        IAttributePusher SetAttribute(Guid attributeId);
    }
}
