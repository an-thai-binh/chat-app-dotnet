using ChatAppApi.Hubs;
using ChatAppApi.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace ChatAppApi.Services
{
    public class FriendshipService
    {
        private readonly AppDbContext _dbContext;
        private readonly IHubContext<ApplicationHub> _hubContext;

        public FriendshipService(AppDbContext dbContext, IHubContext<ApplicationHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        public async Task SendFriendRequestAsync(string? fromId, string toId)
        {
            if(string.IsNullOrEmpty(fromId))
            {
                // log lỗi, gửi lỗi
                return;
            }
            // thêm vào DB

            // gửi thông báo
            await _hubContext.Clients.User(toId).SendAsync("ReceiveFriendRequest", fromId);
        }

        public async Task AcceptFriendRequestAsync(string fromId, string? toId)
        {
            if(string.IsNullOrEmpty(toId))
            {
                // log lỗi, gửi lỗi
                return;
            }
            // lấy ra từ DB


            // chỉnh status

            // gửi thông báo
            await _hubContext.Clients.User(fromId).SendAsync("AcceptFriendRequest", toId);
            await _hubContext.Clients.User(toId).SendAsync("AcceptFriendRequest", fromId);
        }
    }
}