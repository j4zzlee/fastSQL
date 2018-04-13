using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace FastSQL.Magento1
{
    public class FastProvider : BaseProvider
    {
        public override string Id => "1064aasfsfekj2kf2k2kk23fa1e7e401a99239f89362";
        public override string Name => "Magento 1 SOAP API";

        public override string DisplayName => "Magento 1 SOAP API";

        public override string Description => "Magento 1 SOAP API";
        

        public FastProvider(ProviderOptionManager options): base (options)
        {
        }
    }
}
