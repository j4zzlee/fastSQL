using FastSQL.Core;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;

namespace FastSQL.MsAccess
{
    public class ConnectionStringBuilder
    {
        private IEnumerable<OptionItem> _selfOptions;

        public ConnectionStringBuilder(IEnumerable<OptionItem> selfOptions)
        {
            _selfOptions = selfOptions;
        }

        public string Build()
        {
            var filePath = _selfOptions.FirstOrDefault(o => o.Name == "db_path")?.Value;
            var username = _selfOptions.FirstOrDefault(o => o.Name == "UserID")?.Value;
            username = !string.IsNullOrWhiteSpace(username) ? username : "Admin";
            var password = _selfOptions.FirstOrDefault(o => o.Name == "Password")?.Value;
            var options = new List<string>
            {
                "Driver={Microsoft Access Driver (*.mdb, *.accdb)}",
                $"Dbq={filePath}",
                $"Uid={username}",
                $"Pwd={password}",
                "ExtendedAnsiSQL=1",
                //"Exclusive=1"
            };
            return string.Join(";", options);
        }
    }
}
