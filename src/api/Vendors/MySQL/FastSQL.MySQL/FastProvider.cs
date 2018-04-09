using FastSQL.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace FastSQL.MySQL
{
    public class FastProvider : BaseProvider
    {
        public override string Id => "1064aecb081027138f91e1e7e401a99239f89283";
        public override string Name => "MySQL";

        public override string DisplayName => "MySQL";

        public override string Description => "MySQL";
        
        public FastProvider(ProviderOptionManager options): base(options)
        {
        }
    }
}
