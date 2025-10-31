using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EV_SCMMS.Core.Application.DTOs.ChatBot;

namespace EV_SCMMS.Core.Application.Interfaces.Services
{
    public interface IChatBotService
    {
        public Task<JsonNode?> GetResponseAsync(ChatBotRequestDTO request);
    }
}