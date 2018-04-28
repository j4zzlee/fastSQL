using FastSQL.Core;
using System;
using System.Data;

namespace FastSQL.Magento1
{
    public class FastAdapter : BaseAdapter
    {
        //protected FastAdapter(IRichProvider provider) : base(provider)
        //{
        //}

        private readonly Magento1Soap api;

        public FastAdapter(FastProvider provider, Magento1Soap api) : base(provider)
        {
            this.api = api;
        }

        public override bool TryConnect(out string message)
        {
            try
            {
                message = "Connected.";
                api.SetOptions(Options);
                var sessionId = api.Connect();
                return true;
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
