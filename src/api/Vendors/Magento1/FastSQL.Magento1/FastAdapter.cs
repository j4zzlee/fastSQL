using FastSQL.Core;
using FastSQL.Magento1.Magento1Soap;
using System;
using System.Data;

namespace FastSQL.Magento1
{
    public class FastAdapter : BaseAdapter
    {
        private readonly SoapM1 api;

        public FastAdapter(FastProvider provider, SoapM1 api) : base(provider)
        {
            this.api = api;
        }

        public override bool TryConnect(out string message)
        {
            try
            {
                message = "Connected.";
                api.SetOptions(Options);
                return api.TryConnect();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return false;
            }
            finally
            {
                api.End();
                api.Dispose();
            }
        }
    }
}
