using SecureBank.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SecureBank.Helpers
{
    public class OutCalls
    {
        internal static ILogger _logger = LogManager.GetCurrentClassLogger();
        protected HttpClient _client = new HttpClient();
        private readonly string _baseUrl = "";

        public OutCalls(string serviceUrl)
        {
            _baseUrl = serviceUrl;
            _client.Timeout = TimeSpan.FromSeconds(160);
        }

        public void SetAuthenticationHeaderValue(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void SetTokenHeaderValue(string value)
        {
            _client.DefaultRequestHeaders.Add("Token", value);
        }

        public async Task<T> PostData<T>(string url, object obj)
        {
            try
            {
                string postData = JsonConvert.SerializeObject(obj);
                HttpResponseMessage response = await _client.PostAsync(
                    _baseUrl + url,
                    new StringContent(postData, Encoding.UTF8, "application/json"));
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
                else
                {
                    _logger.Error($"Url: {_baseUrl}{url}, ResponseCode {response.StatusCode}, Error {responseContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Url: {_baseUrl}{url} exception {ex}");
            }

            return default;
        }

        public async Task<T> GetData<T>(string url)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync(_baseUrl + url);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
                else
                {
                    _logger.Error($"Url: {_baseUrl}{url}, ResponseCode {response.StatusCode}, Error {responseContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Url: {_baseUrl}{url} exception {ex}");
            }

            return default;
        }
    }
}
