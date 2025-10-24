using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace EV_SCMMS.Core.Application.DTOs.ChatBot
{
    public class ChatBotResponseDTO
    {
        public string ResponseText { get; set; } = string.Empty;
        public List<string> SuggestedActions { get; set; } = new List<string>();
        public bool IsSuccessful { get; set; } = false;
        public JsonElement AdditionalData { get; set; } = JsonDocument.Parse("{}").RootElement;

    }
}