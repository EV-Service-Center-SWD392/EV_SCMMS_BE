using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EV_SCMMS.Core.Application.DTOs.ChatBot;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace EV_SCMMS.Infrastructure.Services
{
    public class ChatBotService : IChatBotService
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;

        public ChatBotService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IDistributedCache cache)
        {
            _httpClient = httpClientFactory.CreateClient("ChatBotClient");
            _httpClient.BaseAddress = new Uri(configuration["ChatBotApi:BaseUrl"]);
            _cache = cache;
        }

        private static string GenerateCacheKey(string json)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
            return $"ChatBotResponse_{BitConverter.ToString(bytes).Replace("-", "")}";
        }

        private static bool FindSuccess(JsonNode? node)
        {
            if (node == null) return false;

            // Nếu node hiện tại là object
            if (node is JsonObject obj)
            {
                foreach (var kvp in obj)
                {
                    if (kvp.Key.Equals("success", StringComparison.OrdinalIgnoreCase))
                    {
                        return kvp.Value?.GetValue<bool>() ?? false;
                    }

                    if (FindSuccess(kvp.Value)) return true; // tìm đệ quy
                }
            }

            return false;
        }


        public async Task<JsonNode?> GetResponseAsync(ChatBotRequestDTO request)
        {
            try
            {
                // Serialize request DTO
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                string cacheKey = GenerateCacheKey(json);

                // Check cache first
                var cachedResponse = await _cache.GetStringAsync(cacheKey);
                if (cachedResponse != null)
                {
                    return JsonNode.Parse(cachedResponse);
                }

                // Gửi POST tới AI service
                HttpResponseMessage response = await _httpClient.PostAsync("api/ai/chat", content);

                // Đọc raw JSON
                var rawResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(
                        $"Lỗi từ AI service: {response.StatusCode}. " +
                        $"Chi tiết: {rawResponse}, Request: {json}"
                    );
                }



                // Cache the response for future requests
                await _cache.SetStringAsync(cacheKey, rawResponse, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache for 10 minutes
                });

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(
                        $"Lỗi từ AI service: {response.StatusCode}. " +
                        $"Chi tiết: {rawResponse}, Request: {json}"
                    );
                }

                // Parse JSON linh hoạt
                JsonNode? jsonResponse;
                try
                {
                    jsonResponse = JsonNode.Parse(rawResponse);
                }
                catch (Exception parseEx)
                {
                    throw new ApplicationException("Không thể phân tích phản hồi JSON từ AI service.", parseEx);
                }

                bool isSuccess = FindSuccess(jsonResponse);
                if (isSuccess)
                {
                    await _cache.SetStringAsync(cacheKey, rawResponse, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    });
                    Console.WriteLine($"[CACHE SET] {cacheKey}");
                }
                else
                {
                    Console.WriteLine($"[CACHE SKIP] success = false cho request {cacheKey}");
                }

                return jsonResponse;


            }
            catch (Exception ex)
            {
                throw new ApplicationException("Lỗi khi gọi dịch vụ ChatBot.", ex.InnerException);
            }
        }
    }
}
