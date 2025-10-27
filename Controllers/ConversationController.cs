using ChatAppApi.Dtos;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly ConversationService _converService;

        public ConversationController(ConversationService converService)
        {
            _converService = converService;
        }

        [HttpGet("friendship/{id}")]
        [Authorize(Policy = "ADMIN_OR_OWNER")]
        public async Task<IActionResult> ShowByUserAndFriendAsync(string id, [FromQuery] string friendId)
        {
            ApiResponse<ConversationResponse> apiResponse = await _converService.ShowByUserAndFriendAsync(id, friendId);
            return Ok(apiResponse);
        }
    }
}
