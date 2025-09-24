using ChatAppApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatAppApi.Hubs
{
    public class ApplicationHub : Hub
    {
        private readonly FriendshipService _fsService;

        public ApplicationHub(FriendshipService fsService)
        {
            _fsService = fsService;
        }

        [Authorize]
        [Authorize(Policy = "ROLE_USER")]
        public async Task SendFriendRequest(string toId)
        {
            await _fsService.SendFriendRequestAsync(Context.UserIdentifier ?? "", toId);
        }

        [Authorize]
        [Authorize(Policy = "ROLE_USER")]
        public async Task AcceptFriendRequest(string fromId)
        {
            await _fsService.AcceptFriendRequestAsync(fromId, Context.UserIdentifier ?? "");
        }

        [Authorize]
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
