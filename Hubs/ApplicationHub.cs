using ChatAppApi.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatAppApi.Hubs
{
    public class ApplicationHub : Hub
    {
        private readonly FriendshipService _fsService;

        public ApplicationHub(FriendshipService fsService)
        {
            _fsService = fsService;
        }

        public async Task SendFriendRequest(string toId)
        {
            await _fsService.SendFriendRequestAsync(Context.UserIdentifier ?? "", toId);
        }

        public async Task AcceptFriendRequest(string fromId)
        {
            await _fsService.AcceptFriendRequestAsync(fromId, Context.UserIdentifier ?? "");
        }

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
