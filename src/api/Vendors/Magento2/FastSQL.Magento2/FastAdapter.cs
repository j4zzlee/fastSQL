using FastSQL.Core;
using System;
using System.Data;
using System.Threading.Tasks;

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
            Task runner = null;
            try
            {
                message = "Connected.";
                api.SetOptions(Options);
                runner = api.Connect();
                runner.Wait();
                return true;
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return false;
            }
            finally
            {
                runner?.Dispose();
            }
        }
    }
}
