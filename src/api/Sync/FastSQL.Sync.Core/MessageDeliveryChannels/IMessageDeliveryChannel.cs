using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Sync.Core.MessageDeliveryChannels
{
    public interface IMessageDeliveryChannel: IOptionManager
    {
        string Id { get; }
        string Name { get; }
        IMessageDeliveryChannel OnReport(Action<string> reporter);
        Task DeliverMessage(string message, MessageType messageType);
    }
}
