using ChatAppApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ChatAppApi.Requirements
{
    public class AdminOrOwnerRequirement : IAuthorizationRequirement { }

    public class AdminOrOwnerHandler : AuthorizationHandler<AdminOrOwnerRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminOrOwnerHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrOwnerRequirement requirement)
        {
            ClaimsPrincipal claimsPrincipal = context.User; // context.User là claims principal từ token
            foreach (Claim claim in claimsPrincipal.Claims)
            {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }
            string? userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userId))
            {
                return Task.CompletedTask;
            }
            string? scope = claimsPrincipal.FindFirst("scope")?.Value;
            if (string.IsNullOrEmpty(scope))
            {
                return Task.CompletedTask;
            }
            // nếu là admin thì cho phép
            if(scope.Contains("ROLE_ADMIN"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            // nếu có id tương ứng với id tài nguyên thì cho phép
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            string? routeId = httpContext?.GetRouteData()?.Values["id"]?.ToString();
            if(routeId != null && routeId == userId)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
