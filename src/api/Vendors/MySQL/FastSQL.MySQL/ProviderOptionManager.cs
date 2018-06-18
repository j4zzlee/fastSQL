using FastSQL.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FastSQL.MySQL
{
    public class ProviderOptionManager : BaseOptionManager
    {
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>
            {
                new OptionItem
                {
                    Name = "Server",
                    DisplayName = "Server",
                    Type = OptionType.Text,
                    Value = "(localhost)"
                },
                new OptionItem
                {
                    Name = "Port",
                    DisplayName = "Port",
                    Type = OptionType.Text,
                    Value = "3306"
                },
                new OptionItem
                {
                    Name = "UserID",
                    DisplayName = "User ID",
                    Type = OptionType.Text,
                    Value = "root"
                },
                new OptionItem
                {
                    Name = "Password",
                    DisplayName = "Password",
                    Type = OptionType.Password,
                    Value = ""
                },
                new OptionItem
                {
                    Name = "Database",
                    DisplayName = "Database",
                    Type = OptionType.Text,
                    Value = ""
                },
                new OptionItem
                {
                    Name = "SslMode",
                    DisplayName = "SslMode",
                    Type = OptionType.List,
                    Value = "",
                    Source = new OptionItemSource
                    {
                        Source = Enum.GetValues(typeof(MySqlSslMode))
                            .Cast<MySqlSslMode>()
                            .Select(v => new { Key = v.ToString(), Value = v.ToString()}),
                        KeyColumnName = "Key",
                        DisplayColumnName = "Value"
                    }
                }
            };
        }
    }
}
