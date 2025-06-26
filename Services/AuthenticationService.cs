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
        private readonly UserRepository _userRepo;

        public AuthenticationService(JwtUtils jwtUtils, UserRepository userRepo)
        {
            _jwtUtils = jwtUtils;
            _userRepo = userRepo;
        }

        public async Task<ApiResponse<AuthenticationResponse>> Login(UserLoginRequest request)
        {
            User? user = await _userRepo.FindByIdentifier(request.Identifier) ?? throw new AppException(ErrorCode.UserNotFound);
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                throw new AppException(ErrorCode.WrongPassword);
            }
            AuthenticationResponse authenticationResponse = new AuthenticationResponse
            {
                AccessToken = _jwtUtils.GenerateAccessToken(user)
            };
            return ApiResponse<AuthenticationResponse>.CreateSuccess(authenticationResponse);
        }

        public async Task<ApiResponse<object?>> Introspect(string token)
        {
            ClaimsPrincipal? claimsPrincipal = _jwtUtils.ValidateToken(token) ?? throw new AppException(ErrorCode.InvalidToken);
            return ApiResponse<object?>.CreateSuccess(null, "Valid token");
        }
    }
}
