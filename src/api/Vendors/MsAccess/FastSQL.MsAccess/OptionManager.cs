using FastSQL.Core;
using System;
using System.Collections.Generic;

namespace FastSQL.MsAccess
{
    public class OptionManager : IOptionManager
    {
        public IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>
            {
                new OptionItem
                {
                    Name = "db_path",
                    DisplayName = "Database File",
                    Type = OptionType.File,
                    Value = "C:\\accdb.db"
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
                }
            };
        }
    }
}
