using ChatAppApi.Dtos;
using ChatAppApi.Dtos.Requests;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authService;

        public AuthenticationController(AuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            ApiResponse<AuthenticationResponse> apiResponse = await _authService.Login(request);
            return Ok(apiResponse);
        }

        [HttpPost("introspect")]
        public async Task<IActionResult> Introspect([FromBody] UserTokenRequest request)
        {
            ApiResponse<object?> apiResponse = await _authService.Introspect(request);
            return Ok(apiResponse);
        }
    }
}
