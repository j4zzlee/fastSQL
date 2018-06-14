using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FastSQL.MsSql
{
    public class ProviderOptionManager : BaseOptionManager
    {
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>
            {
                new OptionItem
                {
                    Name = "DataSource",
                    DisplayName = "Data Source",
                    Type = OptionType.Text,
                    Value = ".\\SQLEXPRESS"
                },
                new OptionItem
                {
                    Name = "UserID",
                    DisplayName = "User ID",
                    Type = OptionType.Text,
                    Value = "sa"
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
                }
            };
        }
    }
}
