using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using System;

namespace FastSQL.Sync.Core.Puller
{
    public interface IPuller : IOptionManager
    {
        IPuller OnReport(Action<string> reporter);
        IPuller SetIndex(IIndexModel model);

        IPuller Init();
        bool Initialized();

        PullResult Preview();
        PullResult PullNext(object lastToken = null);
    }

    public interface IEntityPuller : IPuller
    {
        bool IsImplemented(string processorId, string providerId);
    }

    public interface IAttributePuller : IPuller
    {
        bool IsImplemented(string attributeProcessorId, string entityProcessorId, string providerId);
    }
}
