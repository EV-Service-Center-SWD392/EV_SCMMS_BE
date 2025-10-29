using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EV_SCMMS.Core.Application.DTOs.ChatBot;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace EV_SCMMS.Infrastructure.Services
{
    public class ChatBotService : IChatBotService
    {
        private readonly HttpClient _httpClient;

        public ChatBotService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ChatBotClient");
            _httpClient.BaseAddress = new Uri(configuration["ChatBotApi:BaseUrl"]);
        }

        public async Task<JsonNode?> GetResponseAsync(ChatBotRequestDTO request)
        {
            try
            {
                // Serialize request DTO
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi POST tới AI service
                HttpResponseMessage response = await _httpClient.PostAsync("api/ai/chat", content);

                // Đọc raw JSON
                var rawResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(
                        $"Lỗi từ AI service: {response.StatusCode}. " +
                        $"Chi tiết: {rawResponse}, Request: {json}"
                    );
                }

                // Parse JSON linh hoạt
                try
                {
                    var jsonResponse = JsonNode.Parse(rawResponse);
                    return jsonResponse;
                }
                catch (Exception parseEx)
                {
                    throw new ApplicationException("Không thể phân tích phản hồi JSON từ AI service.", parseEx);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Lỗi khi gọi dịch vụ ChatBot.", ex);
            }
        }
    }
}
