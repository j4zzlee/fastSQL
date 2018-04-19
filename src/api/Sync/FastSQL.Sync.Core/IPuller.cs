using FastSQL.Core;
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
    }

    public interface IAttributePuller : IPuller
    {
        IAttributePuller SetAttribute(Guid attributeId);
        IProcessor GetAttributeProcessor();
        IProcessor GetEntityProcessor();
        
        bool IsImplemented(string attributeProcessorId, string entityProcessorId, string providerId);
    }
}
