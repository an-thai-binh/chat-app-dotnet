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
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notifService;

        public NotificationController(NotificationService notifService)
        {
            _notifService = notifService;
        }

        [HttpGet("latest/{quantity}")]
        [Authorize]
        [Authorize(Policy = "ROLE_USER")]
        public async Task<IActionResult> IndexLatestNotifications(int quantity)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            ApiResponse<List<NotificationResponse>> apiResponse = await _notifService.GetLatestNotificationsAsync(userId, quantity);
            return Ok(apiResponse);
        }

        [HttpPost("maskAsRead")]
        [Authorize]
        [Authorize(Policy = "ROLE_USER")]
        public async Task<IActionResult> MaskAsRead()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            ApiResponse<object> apiResponse = await _notifService.MaskReadForUnreadNotificationsAsync(userId);
            return Ok(apiResponse);
        }
    }
}
