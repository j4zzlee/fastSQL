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
    public class ParentCategoryMapper : BaseMapper
    {
        public ParentCategoryMapper(
            ParentCategoryProcessor processor,
            ParentCategoryMapperOptionManager optionManager,
            FastAdapter adapter) : base(processor, optionManager, adapter)
        {
        }

        public override MapResult Pull(object lastToken = null)
        {
            throw new NotImplementedException();
        }
    }
}
