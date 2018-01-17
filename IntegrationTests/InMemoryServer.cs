using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace IntegrationTests
{
    class InMemoryServer : IDisposable
    {
        private string _url = "http://www.in-memory-server.com";

        private HttpServer _server;
        private HttpClient _client;

        public InMemoryServer()
        {
            var config = new HttpConfiguration();
            refactor_me.WebApiConfig.Register(config);

            _server = new HttpServer(config);
            _client = new HttpClient(_server);
        }

        internal async Task<T> SendRequest<T>(string url, HttpMethod method)
        {
            var response = _client.SendAsync(createRequest(url, method));
            return await response.Result.Content.ReadAsAsync<T>();
        }

        private HttpRequestMessage createRequest(string url, HttpMethod method)
        {
            var request = new HttpRequestMessage();

            request.RequestUri = new Uri(_url + url);
            request.Method = method;

            return request;
        }

        private HttpRequestMessage createRequest<T>(string url, HttpMethod method, T content, MediaTypeFormatter formatter) where T : class
        {
            HttpRequestMessage request = createRequest(url, method);
            request.Content = new ObjectContent<T>(content, formatter);

            return request;
        }

        public void Dispose()
        {
            if (_server != null)
            {
                _server.Dispose();
            }
        }
    }
}
