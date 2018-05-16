using FastSQL.Core;
using FastSQL.Sync.Core.Indexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastSQL.Magento2.Integration.Indexers
{
    public class OrderItemIndexerOptionManager : EntityIndexerOptionManager
    {
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return base.GetOptionsTemplate().Union(new List<OptionItem>());
        }
    }
}
