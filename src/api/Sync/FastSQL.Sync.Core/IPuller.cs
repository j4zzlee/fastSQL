using FastSQL.Core;
using System;

namespace FastSQL.Sync.Core
{
    public interface IPuller : IProcessorVerifier, IRichProviderVerifier, IOptionManager
    {
        PullResult PullNext(object lastToken = null);
    }

    public interface IEntityPuller : IPuller
    {
        IEntityPuller SetEntity(Guid entityId);
    }

    public interface IAttributePuller : IPuller
    {
        IProcessor GetEntityProcessor();
        bool IsEntityProcessor(string id);
        bool IsEntityProcessor(IProcessor processor);
        IAttributePuller SetAttribute(Guid attributeId);
    }
}
