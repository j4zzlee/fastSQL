using FastSQL.Core;
using System;
using System.Data;

namespace FastSQL.Magento2
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
