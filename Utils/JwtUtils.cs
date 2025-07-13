using ChatAppApi.Exceptions;
using ChatAppApi.Models;
using ChatAppApi.Services;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatAppApi.Utils
{
    public class JwtUtils
    {
        private readonly ILogger<JwtUtils> _logger;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly RedisService _redisService;

        public JwtUtils(ILogger<JwtUtils> logger, RedisService redisService)
        {
            _logger = logger;
            _secretKey = Env.GetString("JWT__SECRETKEY") ?? throw new Exception("JWT secret key not found");
            _issuer = Env.GetString("JWT__ISSUER") ?? throw new Exception("JWT issuer not found");
            _redisService = redisService;
        }

        public string GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            string scope = BuildScope(user);

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("display", user.DisplayName),
                new Claim("scope", scope)
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
             );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("type", "refresh")
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public async Task<ClaimsPrincipal?> ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken validatedToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParams, out validatedToken);
                string? logout = await _redisService.GetStringAsync(principal.FindFirst("jti")?.Value ?? "");
                if(logout != null)
                {
                    return null;
                }
                return principal;
            } catch(Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        /// <summary>
        /// Xây claim scope theo chuẩn OAuth2
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Chuỗi scope</returns>
        private string BuildScope(User user)
        {
            StringBuilder result = new StringBuilder();
            foreach(Role role in user.Roles)
            {
                result.Append(role.Name + " ");
                foreach(Permission permission in role.Permissions)
                {
                    result.Append(permission.Name + " ");
                }
            }
            return result.ToString().Trim();
        }
    }
}
