using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Mapper;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Magento1.Integration.Mappers
{
    public class AttributeSetMapper : BaseMapper
    {
        public AttributeSetMapper(
            AttributeSetProcessor processor, 
            AttributeSetMapperOptionManager optionManager, 
            FastProvider provider, 
            FastAdapter adapter, 
            EntityRepository entityRepository, 
            ConnectionRepository connectionRepository) : base(processor, optionManager, provider, adapter, entityRepository, connectionRepository)
        {
        }

        public override MapResult Pull(object lastToken = null)
        {
            throw new NotImplementedException();
        }
    }
}
