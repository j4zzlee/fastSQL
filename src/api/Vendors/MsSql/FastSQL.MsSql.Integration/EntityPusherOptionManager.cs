using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.MsSql.Integration
{
    public class EntityPusherOptionManager : BaseOptionManager
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
                    Name = "pusher_check_exists_script",
                    DisplayName = "Check Exists SQL",
                    OptionGroupNames = new List<string> { "Pusher" },
                    Type = OptionType.Sql,
                    Description = @"SQL query to check if an item is exists or not in destination.",
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
                    Name = "pusher_create_script",
                    DisplayName = "Create SQL",
                    OptionGroupNames = new List<string> { "Pusher" },
                    Type = OptionType.Sql,
                    Description = @"SQL to create an item into destination database.",
                    Example = $@"INSERT INTO [TABLE](COLUMN1, COLUMN2) VALUES (@Value1, @Value2)",
                    Value = $@"INSERT INTO [TABLE](COLUMN1, COLUMN2) VALUES (@Value1, @Value2)"
                },
                new OptionItem
                {
                    Name = "pusher_update_script",
                    DisplayName = "Update SQL",
                    OptionGroupNames = new List<string> { "Pusher" },
                    Type = OptionType.Sql,
                    Description = @"SQL to update item of destination database.",
                    Example = $@"UPDATE [Table]
SET COLUMN1 = @Value1,
COLUMN2 = @Value2
WHERE Id = @Id",
                    Value = $@"UPDATE [Table]
SET COLUMN1 = @Value1,
COLUMN2 = @Value2
WHERE Id = @Id"
                },
                new OptionItem
                {
                    Name = "pusher_delete_script",
                    DisplayName = "Delete SQL",
                    OptionGroupNames = new List<string> { "Pusher" },
                    Type = OptionType.Sql,
                    Description = @"SQL to delete item of destination database.",
                    Example = $@"DELETE FROM [Table]
WHERE Id = @Id",
                    Value = $@"DELETE FROM [Table]
WHERE Id = @Id"
                }
            };
        }
    }
}
