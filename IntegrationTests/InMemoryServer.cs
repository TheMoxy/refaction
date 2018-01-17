using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using refactor_me.Models;

namespace IntegrationTests
{
    class InMemoryServer : IDisposable
    {
        internal static string TestDatabasePath;

        private string _url = "http://www.in-memory-server.com";

        private HttpServer _server;
        private HttpClient _client;

        public InMemoryServer()
        {
            var config = new HttpConfiguration();
            refactor_me.WebApiConfig.Register(config);

            // Clone test database for this run
            Helpers.DataDirectory = TestDatabasePath;

            _server = new HttpServer(config);
            _client = new HttpClient(_server);
        }

        internal async Task<T> SendRequest<T>(string url, HttpMethod method)
        {
            var response = _client.SendAsync(createRequest(url, method));
            return await response.Result.Content.ReadAsAsync<T>();
        }

        internal async Task SendRequest(string url, HttpMethod method)
        {
            await _client.SendAsync(createRequest(url, method));
        }

        internal async Task SendRequest<T>(string url, HttpMethod method, T content)
        {
            await _client.SendAsync(createRequest(url, method, content, new JsonMediaTypeFormatter()));
        }

        private HttpRequestMessage createRequest(string url, HttpMethod method)
        {
            var request = new HttpRequestMessage();

            request.RequestUri = new Uri(_url + url);
            request.Method = method;

            return request;
        }

        private HttpRequestMessage createRequest<T>(string url, HttpMethod method, T content, MediaTypeFormatter formatter)
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
                //Helpers.ClearConnections();
                //System.Threading.Thread.Sleep(1000);
                //System.IO.File.Delete(System.IO.Path.Combine(TestDatabasePath, "Database.mdf"));
            }
        }
    }
}
