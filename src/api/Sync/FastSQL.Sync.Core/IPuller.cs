using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using System;

namespace FastSQL.Sync.Core
{
    public interface IPuller : IOptionManager
    {
        PullResult PullNext(object lastToken = null);
        IRichProvider GetProvider();
    }

    public interface IEntityPuller : IPuller
    {
        IEntityPuller SetEntity(Guid entityId);
        IProcessor GetProcessor();
        bool IsImplemented(string processorId, string providerId);
        IEntityPuller SetEntity(EntityModel entity);
    }

    public interface IAttributePuller : IPuller
    {
        IAttributePuller SetAttribute(Guid attributeId);
        IAttributePuller SetAttribute(AttributeModel attribute, EntityModel entityModel = null);
        IProcessor GetAttributeProcessor();
        IProcessor GetEntityProcessor();
        
        bool IsImplemented(string attributeProcessorId, string entityProcessorId, string providerId);
    }
}
