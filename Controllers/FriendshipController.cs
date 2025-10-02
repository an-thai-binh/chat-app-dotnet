using ChatAppApi.Dtos;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipController : ControllerBase
    {
        private readonly FriendshipService _fsService;

        public FriendshipController(FriendshipService fsService)
        {
            _fsService = fsService;
        }

        [HttpGet("friendRequests")]
        [Authorize]
        [Authorize(Policy = "ROLE_USER")]
        public async Task<IActionResult> IndexFriendRequest()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            ApiResponse<List<FriendRequestResponse>> apiResponse = await _fsService.GetFriendRequestsAsync(userId);
            return Ok(apiResponse);
        }

        [HttpGet("userFriends/{id}")]
        [Authorize]
        [Authorize(Policy = "ADMIN_OR_OWNER")]
        public async Task<IActionResult> IndexUserFriends(string id)
        {
            ApiResponse<List<FriendResponse>> apiResponse = await _fsService.GetUserFriends(id);
            return Ok(apiResponse);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [Authorize(Policy = "ADMIN_OR_OWNER")]
        public async Task<IActionResult> DeleteFriendship(string id, [FromQuery] string friendId)
        {
            ApiResponse<object> apiResponse = await _fsService.DeleteFriendshipAsync(id, friendId);
            return Ok(apiResponse);
        }
    }
}
