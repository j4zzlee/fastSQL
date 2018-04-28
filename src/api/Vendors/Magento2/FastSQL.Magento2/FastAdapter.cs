using FastSQL.Core;
using System;
using System.Data;

namespace FastSQL.Magento2
{
    public class FastAdapter : BaseAdapter
    {
        private readonly Magento2RestApi api;

        public FastAdapter(FastProvider provider, Magento2RestApi api) : base(provider)
        {
            this.api = api;
        }

        public override bool TryConnect(out string message)
        {
            try
            {
                message = "Connected.";
                api.SetOptions(Options);
                var task = api.Connect();
                task.Wait();
                return true;
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return false;
            }
        }
    }
}
