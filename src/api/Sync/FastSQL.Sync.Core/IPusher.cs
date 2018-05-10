using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using System;

namespace FastSQL.Sync.Core
{
    public interface IPusher: IOptionManager
    {
        void Push(Guid itemId);
    }

    public interface IEntityPusher: IPusher
    {
        IEntityPusher SetEntity(EntityModel entity);
        IEntityPusher SetItem(object item);
        IProcessor GetProcessor();
        IRichProvider GetProvider();
        bool IsImplemented(string processorId, string providerId);
    }

    public interface IAttributePusher: IPusher
    {
        IAttributePusher SetAttribute(AttributeModel attribute, EntityModel entity);
        IAttributePusher SetItem(object item);
        IProcessor GetAttributeProcessor();
        IProcessor GetEntityProcessor();
        IRichProvider GetProvider();
        bool IsImplemented(string attributeProcessorId, string entityProcessorId, string providerId);
    }
}
