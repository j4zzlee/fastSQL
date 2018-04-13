using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace FastSQL.Magento2
{
    public class FastProvider : BaseProvider
    {
        public override string Id => "1064aecb081027138f91e1e7e401a9fjkejk2lj92";
        public override string Name => "Magento 2 REST API";

        public override string DisplayName => "Magento 2 REST API";

        public override string Description => "Magento 2 REST API";
        

        public FastProvider(ProviderOptionManager options): base (options)
        {
        }
    }
}
