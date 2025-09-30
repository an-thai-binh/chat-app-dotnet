using ChatAppApi.Dtos;
using ChatAppApi.Dtos.Requests;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/<UserController>
        [HttpGet]
        [Authorize(Policy = "ROLE_ADMIN")]
        public async Task<IActionResult> Index([FromQuery] Pageable pageable)
        {
            ApiResponse<Page<UserResponse>> apiResponse = await _userService.IndexAsync(pageable);
            return Ok(apiResponse);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        [Authorize(Policy = "ADMIN_OR_OWNER")]
        public async Task<IActionResult> Show(string id)
        {
            ApiResponse<UserResponse> apiResponse = await _userService.ShowAsync(id);
            return Ok(apiResponse);
        }

        [HttpGet("profile/{id}")]
        [Authorize(Policy = "ADMIN_OR_OWNER")]
        public async Task<IActionResult> ShowProfile(string id)
        {
            ApiResponse<UserProfileResponse> apiResponse = await _userService.ShowProfileAsync(id);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Tìm kiếm người dùng trong chế độ bạn bè
        /// </summary>
        /// <param name="query">username</param>
        /// <returns>Người dùng tìm thấy</returns>
        [HttpGet("searchUserInFriend")]
        [Authorize(Policy = "ROLE_USER")]
        public async Task<IActionResult> SearchUserInFriend([FromQuery] string query)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? ""; // get sub
            ApiResponse<UserFriendSearchResponse> apiResponse = await _userService.SearchUserInFriendAsync(userId, query);
            return Ok(apiResponse);
        }

        

        // POST api/<UserController>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreationRequest request)
        {
            ApiResponse<UserResponse> apiResponse = await _userService.CreateAsync(request);
            return StatusCode(201, apiResponse);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
