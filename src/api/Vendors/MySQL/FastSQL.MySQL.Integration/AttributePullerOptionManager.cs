using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.MySQL.Integration
{
    public class AttributePullerOptionManager : BaseOptionManager
    {
        public override void Dispose()
        {

        }

        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>
            {
                new OptionItem
                {
                    Name = "puller_sql_script",
                    DisplayName= "SQL Script",
                    OptionGroupNames = new List<string>{ "Puller" },
                    Type = OptionType.Sql,
                    Description = @"SQL Script to get values from Source Database.
Please remember that @Limit & @Offset are required.",
                    Example = $@"SELECT [column1], [column2]
FROM [table]
LIMIT @Limit OFFSET @Offset",
                    Value = $@"SELECT [column1], [column2]
FROM [table]
LIMIT @Limit OFFSET @Offset"
                },
                new OptionItem
                {
                    Name = "puller_page_limit",
                    DisplayName= "@Limit",
                    OptionGroupNames = new List<string>{ "Puller" },
                    Type = OptionType.Text,
                    Description = @"A maximum limitation per page when pull data",
                    Example = $@"100",
                    Value = $@"100"
                }
            };
        }
    }
}
