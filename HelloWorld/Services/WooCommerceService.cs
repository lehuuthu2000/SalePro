using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using helloworld.Models;

namespace helloworld.Services
{
    public class WooCommerceService
    {
        private readonly HttpClient _httpClient;

        public WooCommerceService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> CheckConnectionAsync(string domain, string key, string secret)
        {
            try
            {
                // Thử lấy System Status hoặc 1 product để test
                var url = $"{domain.TrimEnd('/')}/wp-json/wc/v3/system_status";
                var request = CreateRequest(url, key, secret);
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<WcProduct>> GetProductsAsync(string domain, string key, string secret, int page = 1, int perPage = 10)
        {
            var url = $"{domain.TrimEnd('/')}/wp-json/wc/v3/products?page={page}&per_page={perPage}";
            var request = CreateRequest(url, key, secret);
            
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<WcProduct>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return products ?? new List<WcProduct>();
        }

        public async Task<List<WcVariation>> GetProductVariationsAsync(string domain, string key, string secret, int productId)
        {
             var url = $"{domain.TrimEnd('/')}/wp-json/wc/v3/products/{productId}/variations";
             var request = CreateRequest(url, key, secret);
             
             var response = await _httpClient.SendAsync(request);
             // Nếu lỗi 404 nghĩa là không có variations hoặc route sai, nhưng cứ throw để caller xử lý
             response.EnsureSuccessStatusCode();

             var content = await response.Content.ReadAsStringAsync();
             var variations = JsonSerializer.Deserialize<List<WcVariation>>(content, new JsonSerializerOptions
             {
                 PropertyNameCaseInsensitive = true
             });

             return variations ?? new List<WcVariation>();
        }

        private HttpRequestMessage CreateRequest(string url, string key, string secret)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var authString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{key}:{secret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authString);
            return request;
        }
    }
}
