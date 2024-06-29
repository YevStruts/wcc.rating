using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
//using Microservices = wcc.rating.rating.Models.Microservices;

namespace wcc.rating.kernel.Helpers
{
    internal class ApiCaller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiCaller(string baseUrl)
        {
            _httpClient = new HttpClient();
            _baseUrl = baseUrl;
        }

        public async Task<TResponse> GetAsync<TResponse>(string endpoint)
        {
            var url = $"{_baseUrl}/{endpoint}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<TResponse>(responseContent);

            return responseObject;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request)
        {
            var url = $"{_baseUrl}/{endpoint}";

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<TResponse>(responseContent);

            return responseObject;
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var url = $"{_baseUrl}/{endpoint}";

            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}
