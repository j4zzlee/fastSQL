using FastSQL.Core;
using FastSQL.Magento1.Magento1Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace FastSQL.Magento1
{
    public class SoapM1 : IDisposable
    {
        protected string CurrentSession;
        protected string ApiUrl { get; set; }
        protected string ApiUser { get; set; }
        protected string ApiKey { get; set; }

        protected PortTypeClient _client;

        public SoapM1()
        {
        }

        public SoapM1 SetOptions(IEnumerable<OptionItem> options)
        {
            ApiUrl = options.FirstOrDefault(o => o.Name == "api_uri").Value;
            ApiUser = options.FirstOrDefault(o => o.Name == "api_user").Value;
            ApiKey = options.FirstOrDefault(o => o.Name == "api_key").Value;

            return this;
        }

        public PortTypeClient GetClient()
        {
            return new PortTypeClient(ApiUrl.ToLower().Contains("https") ? "HttpsPort" : "HttpPort", ApiUrl);
        }

        public PortTypeClient Begin()
        {
            End();
            _client = GetClient();
            _client?.Open();
            CurrentSession = _client?.login(ApiUser, ApiKey);
            return _client;
        }

        public void End()
        {
            _client?.Close();
            CurrentSession = null;
        }

        public bool TryConnect()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ApiUrl)
                       || string.IsNullOrWhiteSpace(ApiUser) ||
                       string.IsNullOrWhiteSpace(ApiKey))
                {
                    return false;
                }
                Begin();
                return !string.IsNullOrWhiteSpace(CurrentSession);
            }
            finally
            {
                End();
            }
        }

        public string GetSession()
        {
            return CurrentSession;
        }

        public virtual void Dispose()
        {
            _client?.Close();
            CurrentSession = null;
        }
    }
}
