using FastSQL.Core;
using System;
using System.Collections.Generic;

namespace FastSQL.MsAccess
{
    public class ConnectorOptions : IConnectorOptions
    {
        public IEnumerable<OptionItem> GetOptions()
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
                //new OptionItem
                //{
                //    Name = "host",
                //    DisplayName = "Host",
                //    Type = OptionType.Text,
                //    Value = "(localhost)"
                //},
                //new OptionItem
                //{
                //    Name = "port",
                //    DisplayName = "Port",
                //    Type = OptionType.Text,
                //    Value = "3306"
                //},
                //new OptionItem
                //{
                //    Name = "username",
                //    DisplayName = "Username",
                //    Type = OptionType.Text,
                //    Value = "root"
                //},
                //new OptionItem
                //{
                //    Name = "password",
                //    DisplayName = "Password",
                //    Type = OptionType.Password,
                //    Value = ""
                //}
            };
        }
    }
}
