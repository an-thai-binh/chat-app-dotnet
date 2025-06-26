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
        private readonly UserService _userService;

        public AuthenticationController(AuthenticationService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            ApiResponse<AuthenticationResponse> apiResponse = await _authService.Login(request);
            return Ok(apiResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreationRequest request)
        {
            ApiResponse<UserResponse> apiResponse = await _userService.CreateAsync(request);
            return StatusCode(201, apiResponse);
        }

        [HttpPost("introspect")]
        public async Task<IActionResult> Introspect([FromBody] UserTokenRequest request)
        {
            ApiResponse<object?> apiResponse = await _authService.Introspect(request);
            return Ok(apiResponse);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] UserTokenRequest request)
        {
            ApiResponse<AuthenticationResponse> apiResponse = await _authService.Refresh(request);
            return Ok(apiResponse);
        }
    }
}
