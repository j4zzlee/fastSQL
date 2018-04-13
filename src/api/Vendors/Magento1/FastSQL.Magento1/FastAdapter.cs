using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.IO;
using System.Linq;

namespace FastSQL.Magento1
{
    public class FastAdapter : BaseAdapter
    {
        public FastAdapter(FastProvider provider) : base(provider)
        {
        }

        public override bool TryConnect(out string message)
        {
            IDbConnection conn = null;
            try
            {
                message = "Connected.";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
            finally
            {
                conn?.Dispose();
            }
        }
    }
}
