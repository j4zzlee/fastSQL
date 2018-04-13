using FastSQL.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Magento2
{
    public class Magento2RestApi
    {
        private string _apiUri;
        private string _consumerSecret;
        private string _consumerKey;
        private string _accessToken;
        private string _accessTokenSecret;

        private JsonSerializer _jsonSerializer;

        public async Task<object> Connect()
        {
            var response = await ExecuteAsync<object>("categories/461", Method.GET);
            return response;
        }

        private bool _contentTypeHeaderWithUnderscore = false;

        public Magento2RestApi SetOptions(IEnumerable<OptionItem> options)
        {
            _apiUri = options.FirstOrDefault(o => o.Name == "api_uri").Value?.TrimEnd('/');
            _consumerKey = options.FirstOrDefault(o => o.Name == "consumer_key").Value;
            _consumerSecret = options.FirstOrDefault(o => o.Name == "consumer_secret").Value;
            _accessToken = options.FirstOrDefault(o => o.Name == "access_token").Value;
            _accessTokenSecret = options.FirstOrDefault(o => o.Name == "access_token_secret").Value;

            _jsonSerializer = new JsonSerializer();
            
            return this;
        }

        protected RestClient GetClient()
        {
            var client = new RestClient(_apiUri);

            client.AddDefaultHeader(_contentTypeHeaderWithUnderscore ? "Content_Type" : "Content-Type", "application/json");

            client.ClearHandlers();
            client.AddHandler("application/json", _jsonSerializer);
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(
               _consumerKey,
               _consumerSecret,
               _accessToken,
               _accessTokenSecret);
            return client;
        }
        
        protected IRestRequest CreateRequest(string url, Method method = Method.GET)
        {
            var request = new RestRequest
            {
                Resource = url,
                Method = method,
                RequestFormat = DataFormat.Json,
                JsonSerializer = _jsonSerializer
            };
            return request;
        }
        
        protected virtual T GetData<T>(IRestResponse response, Func<JObject, T> resolver)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                if (response.ContentType.ToUpperInvariant().Contains("APPLICATION/JSON"))
                {
                    // 404: the requested item is not found
                    return default(T);
                }
                else
                {
                    // 404: the requested uri is not found
                    throw new Exception($@"The requested resource ""{response.ResponseUri}"" does not exists.");
                }
            }

            if (response.Request.Method == Method.DELETE || !string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                {
                    throw new Exception(response.ErrorMessage ?? response.Content);
                }
            }
            else if (!string.IsNullOrWhiteSpace(response.ErrorMessage) || response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(response.ErrorMessage ?? response.Content);
            }
            else if (response.ErrorException != null)
            {
                throw new Exception("The request was not succesfully completed.", response.ErrorException);
            }
            var resp = JsonConvert.DeserializeObject(response.Content) as JObject;
            return resolver.Invoke(resp);
        }

        protected virtual T GetData<T>(IRestResponse<T> response) where T : new()
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                if (response.ContentType.ToUpperInvariant().Contains("APPLICATION/JSON"))
                {
                    // 404: the requested item is not found
                    return default(T);
                }
                else
                {
                    // 404: the requested uri is not found
                    throw new Exception($@"The requested resource ""{response.ResponseUri}"" does not exists.");
                }
            }

            if (response.Request.Method == Method.DELETE || !string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                {
                    throw new Exception(response.ErrorMessage ?? response.Content);
                }
            }
            else if (!string.IsNullOrWhiteSpace(response.ErrorMessage) || response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(response.ErrorMessage ?? response.Content);
            }
            else if (response.ErrorException != null)
            {
                throw new Exception("The request was not succesfully completed.", response.ErrorException);
            }

            return response.Data;
        }

        public async Task<T> ExecuteAsync<T>(string url, Method method = Method.GET) where T : new()
        {
            var request = CreateRequest(url, method);
            var client = GetClient();
            client.FollowRedirects = request.Method != Method.POST;
            var response = client.Execute<T>(request);
            return await Task.FromResult(GetData(response));
        }

        protected async Task<IRestResponse> ExecuteAsync(string url, Method method = Method.GET)
        {
            var request = CreateRequest(url, method);
            var client = GetClient();
            client.FollowRedirects = request.Method != Method.POST;
            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
    }
}
