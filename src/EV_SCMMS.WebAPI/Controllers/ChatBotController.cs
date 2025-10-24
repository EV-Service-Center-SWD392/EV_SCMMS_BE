using System.Text.Json.Nodes;
using EV_SCMMS.Core.Application.DTOs.ChatBot;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        private readonly IChatBotService _chatBotService;

        public ChatBotController(IChatBotService chatBotService)
        {
            _chatBotService = chatBotService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> GetResponse([FromBody] ChatBotRequestDTO request)
        {
            try
            {
                JsonNode? result = await _chatBotService.GetResponseAsync(request);

                if (result == null)
                    return BadRequest("AI service trả về null.");

                // Trả nguyên JSON về client
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
