using FastSQL.Core;
using Magento1Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace FastSQL.Magento1
{
    public class Magento1Soap : IDisposable
    {
        protected string CurrentSession;
        protected string ApiUrl { get; set; }
        protected string ApiUser { get; set; }
        protected string ApiKey { get; set; }

        protected PortTypeClient _client;
        protected EndpointAddress _address => !string.IsNullOrWhiteSpace(ApiUrl) ? new EndpointAddress(ApiUrl) : null;
        protected BasicHttpBinding _httpBinding => new BasicHttpBinding()
        {
            MaxBufferPoolSize = 20000000,
            MaxReceivedMessageSize = 20000000,
            ReceiveTimeout = new TimeSpan(0, 60, 0),
            OpenTimeout = new TimeSpan(0, 60, 0),
            CloseTimeout = new TimeSpan(0, 60, 0),
            TransferMode = TransferMode.Streamed
        };
        protected BasicHttpsBinding _httpsBinding => new BasicHttpsBinding()
        {
            MaxBufferPoolSize = 20000000,
            MaxReceivedMessageSize = 20000000,
            ReceiveTimeout = new TimeSpan(0, 60, 0),
            OpenTimeout = new TimeSpan(0, 60, 0),
            CloseTimeout = new TimeSpan(0, 60, 0),
            TransferMode = TransferMode.Streamed,
        };

        public Magento1Soap()
        {
        }

        public Magento1Soap SetOptions(IEnumerable<OptionItem> options)
        {
            ApiUrl = options.FirstOrDefault(o => o.Name == "api_uri").Value;
            ApiUser = options.FirstOrDefault(o => o.Name == "api_user").Value;
            ApiKey = options.FirstOrDefault(o => o.Name == "api_key").Value;

            return this;
        }

        protected PortTypeClient GetClient()
        {
            if (ApiUrl.ToLower().Contains("https"))
            {
                return new PortTypeClient(_httpsBinding, _address);
            }
            else
            {
                return new PortTypeClient(_httpBinding, _address);
            }
        }

        public async Task<PortTypeClient> Begin()
        {
            await End();
            _client = GetClient();
            await _client?.OpenAsync();
            CurrentSession = await _client?.loginAsync(ApiUser, ApiKey);
            return _client;
        }

        public async Task End()
        {
            try
            {
                await _client?.CloseAsync();
            }
            catch
            {
                // do nothing
            }
        }

        public async Task<bool> Connect()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ApiUrl)
                       || string.IsNullOrWhiteSpace(ApiUser) ||
                       string.IsNullOrWhiteSpace(ApiKey))
                {
                    return false;
                }
                await Begin();
                return !string.IsNullOrWhiteSpace(CurrentSession);
            }
            finally
            {
                await End();
            }
        }

        public async void Dispose()
        {
            await _client?.CloseAsync();
        }
    }
}
