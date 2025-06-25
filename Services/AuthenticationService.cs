using ChatAppApi.Dtos;
using ChatAppApi.Dtos.Requests;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Models;
using ChatAppApi.Repositories;
using ChatAppApi.Utils;
using System.Security.Claims;

namespace ChatAppApi.Services
{
    public class AuthenticationService
    {
        private readonly UserRepository _userRepo;

        public AuthenticationService(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<ApiResponse<AuthenticationResponse>> Login(UserLoginRequest request)
        {
            User? user = await _userRepo.FindByIdentifier(request.Identifer) ?? throw new Exception();
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                throw new Exception();
            }
            AuthenticationResponse authenticationResponse = new AuthenticationResponse
            {
                AccessToken = JwtUtils.GenerateAccessToken(user)
            };
            return ApiResponse<AuthenticationResponse>.CreateSuccess(authenticationResponse);
        }

        public async Task<ApiResponse<object?>> Introspect(string token)
        {
            ClaimsPrincipal? claimsPrincipal = JwtUtils.ValidateToken(token) ?? throw new Exception();
            return ApiResponse<object?>.CreateSuccess(null, "Valid token");
        }
    }
}
