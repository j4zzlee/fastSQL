using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastSQL.Magento1.Integration.Pushers
{
    public class SubCategoryPusherOptionManager : BaseOptionManager
    { 
        public RepositoryFactory RepositoryFactory { get; set; }
        public SubCategoryPusherOptionManager()
        {
        }

        public override void Dispose()
        {
            RepositoryFactory.Release(this);
        }

        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            using (var entityRepository = RepositoryFactory.Create<EntityRepository>(this))
            {
                return new List<OptionItem> {
                    new OptionItem
                    {
                        Name = "root_category_id",
                        DisplayName = "Root Category Id"
                    },
                    new OptionItem
                    {
                        Name = "is_anchor",
                        DisplayName = "Is Anchor",
                        Type = OptionType.Boolean
                    },
                    new OptionItem
                    {
                        Name = "website_ids",
                        DisplayName = "Website Ids",
                        Value = "1"
                    },
                    new OptionItem
                    {
                        Name = "store_id",
                        DisplayName = "Store Ids",
                        Value = "0"
                    },
                    new OptionItem
                    {
                        Name = "parent_entity_id",
                        DisplayName = "Parent Category Entity",
                        Type = OptionType.List,
                        OptionGroupNames = new List<string> { "Pusher" },
                        Source = new OptionItemSource
                        {
                            Source = entityRepository.GetAll().Cast<object>(),
                            KeyColumnName = "Id",
                            DisplayColumnName = "Name"
                        }
                    },
                };
            }
        }
    }
}
