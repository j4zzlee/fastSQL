using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using System;

namespace FastSQL.Sync.Core
{
    public interface IPusher: IOptionManager
    {
        string Create();
        string Remove(string destinationId = null);
        string Update(string destinationId = null);
        string GetDestinationId();
        void OnReport(Action<string> reporter);
        IPusher SetItem(IndexItemModel item);
    }

    public interface IEntityPusher: IPusher
    {
        IEntityPusher SetEntity(EntityModel entity);
        IProcessor GetProcessor();
        IRichProvider GetProvider();
        bool IsImplemented(string processorId, string providerId);
    }

    public interface IAttributePusher: IPusher
    {
        IAttributePusher SetAttribute(AttributeModel attribute, EntityModel entity);
        IProcessor GetAttributeProcessor();
        IProcessor GetEntityProcessor();
        IRichProvider GetProvider();
        bool IsImplemented(string attributeProcessorId, string entityProcessorId, string providerId);
    }
}
