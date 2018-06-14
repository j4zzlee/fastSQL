using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Mapper
{
    public class EntityMapperOptionManager : BaseOptionManager
    {
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>
            {
                new OptionItem
                {
                    Name = "mapper_id_key",
                    DisplayName = "@Foreign ID Column",
                    Description = @"A column name that is marked as PRIMARY KEY of the pulled data",
                    Value = string.Empty,
                    Example = "id",
                    OptionGroupNames = new List<string>{ "Mapper" },
                },
                new OptionItem
                {
                    Name = "mapper_foreign_keys",
                    DisplayName = "@Foreign Key Columns",
                    Description = @"A comma separated list of columns that are marked as [KEYS] of pulled data. The foreign key can be PRIMARY KEY or any columns that are important 
to check between pull data and matched data",
                    Value = string.Empty,
                    Example = "order_id,product_id",
                    OptionGroupNames = new List<string>{ "Indexer" },
                },
                new OptionItem
                {
                    Name = "mapper_reference_keys",
                    DisplayName = "@Reference Key Columns",
                    Description = @"A comma separated list of columns that are marked as [KEYS] of indexed data and useful for system to track for changes.",
                    Value = string.Empty,
                    Example = "OrderID,ItemID",
                    OptionGroupNames = new List<string>{ "Indexer" },
                }
            };
        }
    }
}
