using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.MsSql.Integration
{
    public class AttributePullerOptionManager : BaseOptionMananger
    {
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
                    Example = $@"SELECT
Col1, Col2, ...
FROM [Table]
WHERE [Conditions]",
                    Value = $@"SELECT
Col1, Col2, ...
FROM [Table]
WHERE [Conditions]"
                },
                new OptionItem
                {
                    Name = "puller_is_sql_2008",
                    DisplayName= "Is SQL Server 2008",
                    Type = OptionType.Boolean,
                    Description = @"Determine if a sql server is 2008 or below version"
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
