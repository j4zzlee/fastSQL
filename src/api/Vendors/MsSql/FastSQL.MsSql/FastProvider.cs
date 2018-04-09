using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace FastSQL.MsSql
{
    public class FastProvider : BaseProvider
    {
        public override string Id => "1064aecb081027138f91e1e7e401a99239f89362";
        public override string Name => "MsSql";

        public override string DisplayName => "Microsoft SQL Server";

        public override string Description => "Microsoft SQL Server";
        

        public FastProvider(ProviderOptionManager options): base (options)
        {
        }
    }
}
