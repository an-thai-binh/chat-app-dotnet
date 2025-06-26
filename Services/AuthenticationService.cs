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

        public async Task<ApiResponse<object?>> Introspect(UserTokenRequest request)
        {
            ClaimsPrincipal? claimsPrincipal = _jwtUtils.ValidateToken(request.Token) ?? throw new AppException(ErrorCode.InvalidToken);
            return ApiResponse<object?>.CreateSuccess(null, "Valid token");
        }
    }
}
