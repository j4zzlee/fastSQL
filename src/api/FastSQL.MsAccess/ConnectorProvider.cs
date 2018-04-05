using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace FastSQL.MsAccess
{
    public class ConnectorProvider : BaseProvider
    {
        public override string Id => "1064aecb081027138f91e1e7e401a99239f8928d";
        public override string Name => "MsAccess";

        public override string DisplayName => "Microsoft Access";

        public override string Description => "Microsoft Access";
        
        public ConnectorProvider(ConnectorOptions options, ConnectorAdapter adapter) : base(options, adapter)
        {
        }
    }
}
