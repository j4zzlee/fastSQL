using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.MySQL.Integration
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
                    Example = $@"SELECT [column1], [column2]
FROM [table]
LIMIT @Limit OFFSET @Offset",
                    Value = $@"SELECT [column1], [column2]
FROM [table]
LIMIT @Limit OFFSET @Offset"
                },
                new OptionItem
                {
                    Name = "puller_id_column",
                    DisplayName = "ID Column",
                    Description = "ID Column name",
                    Value = "ID",
                    Example = "ID",
                    OptionGroupNames = new List<string>{ "Puller" },
                },
                new OptionItem
                {
                    Name = "puller_lookup_columns",
                    DisplayName = "Lookup Columns",
                    Description = "A comma separated list of columns to lookup as values or primary keys",
                    Example = "Value",
                    OptionGroupNames = new List<string>{ "Puller" },
                }
            };
        }
    }
}
