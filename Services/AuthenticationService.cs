using ChatAppApi.Dtos;
using ChatAppApi.Dtos.Requests;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Exceptions;
using ChatAppApi.Models;
using ChatAppApi.Repositories;
using ChatAppApi.Utils;
using System.Security.Claims;

namespace ChatAppApi.Services
{
    public class AuthenticationService
    {
        private readonly JwtUtils _jwtUtils;
        private readonly RedisService _redisService;
        private readonly UserRepository _userRepo;
        public AuthenticationService(JwtUtils jwtUtils, RedisService redisService, UserRepository userRepo)
        {
            _jwtUtils = jwtUtils;
            _redisService = redisService;
            _userRepo = userRepo;
        }

        private async Task<AuthenticationResponse> CreateAuthenticationResponseAsync(User user)
        {
            string refreshToken = _jwtUtils.GenerateRefreshToken(user);
            await _redisService.SetStringAsync("REFRESH_TOKEN:" + user.Id, refreshToken, TimeSpan.FromHours(24));
            AuthenticationResponse authenticationResponse = new AuthenticationResponse
            {
                AccessToken = _jwtUtils.GenerateAccessToken(user),
                RefreshToken = refreshToken
            };
            return authenticationResponse;
        }

        public async Task<ApiResponse<AuthenticationResponse>> Login(UserLoginRequest request)
        {
            User? user = await _userRepo.FindByIdentifier(request.Identifier) ?? throw new AppException(ErrorCode.UserNotFound);
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                throw new AppException(ErrorCode.WrongPassword);
            }
            AuthenticationResponse authenticationResponse = await CreateAuthenticationResponseAsync(user);
            return ApiResponse<AuthenticationResponse>.CreateSuccess(authenticationResponse);
        }

        public async Task<ApiResponse<object?>> Logout(UserTokenRequest request)
        {
            ClaimsPrincipal? claimsPrincipal = await _jwtUtils.ValidateToken(request.Token);
            if(claimsPrincipal == null)
            {
                return ApiResponse<object?>.CreateSuccess(null, "Logout successful (Invalid Token)");
            }
            string userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new AppException(ErrorCode.InvalidToken);  // sub bị ánh xạ thành ClaimTypes.NameIdentifier
            await _redisService.RemoveAsync("REFRESH_TOKEN:" + userId);
            string jti = claimsPrincipal.FindFirst("jti")?.Value ?? throw new AppException(ErrorCode.InvalidToken);
            await _redisService.SetStringAsync(jti, "LOGOUT", TimeSpan.FromHours(1));
            return ApiResponse<object?>.CreateSuccess(null, "Logout successful");
        }

        public async Task<ApiResponse<AuthenticationResponse>> Refresh(UserTokenRequest request)
        {
            ClaimsPrincipal claimsPrincipal = await _jwtUtils.ValidateToken(request.Token) ?? throw new AppException(ErrorCode.InvalidToken);
            string type = claimsPrincipal.FindFirst("type")?.Value ?? "";
            if(type != "refresh")   // nếu không phải refresh token thì token không hợp lệ
            {
                throw new AppException(ErrorCode.InvalidToken);
            }
            User user = await _userRepo.FindByIdentifier(claimsPrincipal.FindFirst("name")?.Value ?? "") ?? throw new AppException(ErrorCode.UserNotFound);
            string storedToken = await _redisService.GetStringAsync("REFRESH_TOKEN:" + user.Id) ?? throw new AppException(ErrorCode.InvalidToken);
            if(storedToken != request.Token) {
                throw new AppException(ErrorCode.InvalidToken);
            }
            AuthenticationResponse authenticationResponse = await CreateAuthenticationResponseAsync(user);
            return ApiResponse<AuthenticationResponse>.CreateSuccess(authenticationResponse);
        }

        public async Task<ApiResponse<object?>> Introspect(UserTokenRequest request)
        {
            ClaimsPrincipal? claimsPrincipal = await _jwtUtils.ValidateToken(request.Token) ?? throw new AppException(ErrorCode.InvalidToken);
            return ApiResponse<object?>.CreateSuccess(null, "Valid token");
        }

    }
}
