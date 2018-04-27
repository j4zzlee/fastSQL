using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.MsSql.Integration
{
    public class EntityPullerOptionManager : BaseOptionMananger
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
                    Example = "Description, ParentID",
                    OptionGroupNames = new List<string>{ "Puller" },
                }
            };
        }
    }
}
