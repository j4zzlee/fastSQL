using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;

namespace FastSQL.Sync.Core.Pusher
{
    public interface IPusher: IOptionManager
    {
        PushState Create(out string destinationId);
        PushState Remove(string destinationId = null);
        PushState Update(string destinationId = null);
        string GetDestinationId();
        IPusher OnReport(Action<string> reporter);
        IPusher SetItem(IndexItemModel item);
        IPusher SetIndex(IIndexModel model);
    }

    public interface IEntityPusher: IPusher
    {
        bool IsImplemented(string processorId, string providerId);
    }

    public interface IAttributePusher: IPusher
    {
        bool IsImplemented(string attributeProcessorId, string entityProcessorId, string providerId);
    }
}
