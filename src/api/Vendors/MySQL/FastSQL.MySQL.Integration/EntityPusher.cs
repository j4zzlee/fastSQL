using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.MySQL.Integration
{
    public class EntityPusher : BaseEntityPusher
    {
        public EntityPusher(
            EntityPusherOptionManager optionManager, 
            EntityProcessor processor,
            FastProvider provider,
            FastAdapter adapter,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository) : base(optionManager, processor, provider, adapter, entityRepository, connectionRepository)
        {
        }

        public override string Create()
        {
            throw new NotImplementedException();
        }

        public override string GetDestinationId()
        {
            throw new NotImplementedException();
        }

        public override string Remove(string destinationId = null)
        {
            throw new NotImplementedException();
        }

        public override string Update(string destinationId = null)
        {
            throw new NotImplementedException();
        }
    }
}
