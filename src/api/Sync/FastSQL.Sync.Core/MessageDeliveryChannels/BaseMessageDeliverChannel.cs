using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Sync.Core.MessageDeliveryChannels
{
    public abstract class BaseMessageDeliverChannel: IMessageDeliveryChannel
    {
        protected readonly IOptionManager OptionManager;
        private Action<string> _reporter;

        public virtual IEnumerable<OptionItem> Options => OptionManager.Options;

        public abstract string Id { get; }
        public abstract string Name { get; }

        public BaseMessageDeliverChannel(IOptionManager optionManager)
        {
            OptionManager = optionManager;
        }

        public virtual IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return OptionManager.GetOptionsTemplate();
        }

        public IMessageDeliveryChannel OnReport(Action<string> reporter)
        {
            _reporter = reporter;
            return this;
        }

        public IMessageDeliveryChannel Report(string message)
        {
            _reporter?.Invoke(message);
            return this;
        }

        public virtual IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            return OptionManager.SetOptions(options);
        }

        public abstract Task DeliverMessage(string message, MessageType messageType);

        public virtual void Dispose()
        {

        }
    }
}
