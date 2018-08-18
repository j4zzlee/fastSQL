using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Magento1.Integration.Pushers
{
    public class ProductPusherOptionManager : BaseOptionManager
    {
        private readonly EntityRepository entityRepository;

        public ProductPusherOptionManager(EntityRepository entityRepository)
        {
            this.entityRepository = entityRepository;
        }
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            var entities = entityRepository.GetAll();
            return new List<OptionItem> {
                new OptionItem
                {
                    Name = "website_ids",
                    DisplayName = "@Website Ids",
                    Type = OptionType.Text,
                    Value = "1"
                },
                new OptionItem
                {
                    Name = "store_ids",
                    DisplayName = "@Store Ids",
                    Type = OptionType.Text,
                    Value = "0"
                }
            };
        }
    }
}
