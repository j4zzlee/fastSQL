using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Pusher;
using System;

namespace FastSQL.Magento2.Integration.Pushers
{
    public class SubCategoryPusher : BaseEntityPusher
    {
        public SubCategoryPusher(SubCategoryPusherOptionManager optionManager,
            SubCategoryProcessor processor,
            FastProvider provider,
            FastAdapter adapter) : base(optionManager, processor, provider, adapter)
        {
        }

        public override PushState Create(out string destinationId)
        {
            throw new NotImplementedException();
        }

        public override string GetDestinationId()
        {
            throw new NotImplementedException();
        }

        public override PushState Remove(string destinationId = null)
        {
            throw new NotImplementedException();
        }

        public override PushState Update(string destinationId = null)
        {
            throw new NotImplementedException();
        }
    }
}
