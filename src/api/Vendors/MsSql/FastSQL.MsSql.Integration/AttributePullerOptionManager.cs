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
                    Example = $@";WITH Results_CTE AS
(
    SELECT
        Col1, Col2, ...,
        ROW_NUMBER() OVER (ORDER BY ID, ...) AS RowNum
    FROM [Table]
    WHERE [Conditions]
)
SELECT Col1, Col2, ...
FROM Results_CTE
WHERE RowNum >= @Offset
AND RowNum < @Offset + @Limit",
                    Value = $@";WITH Results_CTE AS
(
    SELECT
        Col1, Col2, ...,
        ROW_NUMBER() OVER (ORDER BY ID, ...) AS RowNum
    FROM [Table]
    WHERE [Conditions]
)
SELECT Col1, Col2, ...
FROM Results_CTE
WHERE RowNum >= @Offset
AND RowNum < @Offset + @Limit"
                },
                new OptionItem
                {
                    Name = "puller_page_limit",
                    DisplayName= "@Limit",
                    OptionGroupNames = new List<string>{ "Puller" },
                    Type = OptionType.Sql,
                    Description = @"A maximum limitation per page when pull data",
                    Example = $@"100",
                    Value = $@"100"
                }
            };
        }
    }
}
