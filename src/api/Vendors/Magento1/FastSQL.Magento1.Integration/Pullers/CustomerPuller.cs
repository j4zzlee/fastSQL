using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Magento1.Integration.Pullers
{
    public class CustomerPuller : BaseEntityPuller
    {
        public CustomerPuller(CustomerPullerOptionManager optionManager,
            CustomerProcessor processor,
            FastAdapter adapter) : base(optionManager, processor, adapter)
        {
        }

        public override IPuller Init()
        {
            return this;
        }

        public override bool Initialized()
        {
            return true;
        }

        public override PullResult Preview()
        {
            throw new NotImplementedException();
        }

        public override PullResult PullNext(object lastToken = null)
        {
            throw new NotImplementedException();
        }
    }
}
