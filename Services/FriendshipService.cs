using AutoMapper;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Hubs;
using ChatAppApi.Models;
using ChatAppApi.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace ChatAppApi.Services
{
    public class FriendshipService
    {
        private readonly ILogger<FriendshipService> _logger;
        private readonly IMapper _mapper;
        private readonly IHubContext<ApplicationHub> _hubContext;
        private readonly FriendshipRepository _fsRepo;
        private readonly UserRepository _userRepo;

        public FriendshipService(ILogger<FriendshipService> logger, IMapper mapper, IHubContext<ApplicationHub> hubContext, FriendshipRepository fsRepo, UserRepository userRepo)
        {
            _logger = logger;
            _mapper = mapper;
            _hubContext = hubContext;
            _fsRepo = fsRepo;
            _userRepo = userRepo;
        }

        public async Task SendFriendRequestAsync(string fromId, string toId)
        {
            User? sender = await _userRepo.FindByIdAsync(fromId);
            if(sender == null)
            {
                _logger.LogWarning("SendFriendRequest Failed: Sender not found");
                return;
            }
            User? receiver = await _userRepo.FindByIdAsync(toId);
            if(receiver == null)
            {
                _logger.LogWarning("SendFriendRequest Failed: Receiver not found");
                return;
            }
            Friendship friendship = new Friendship
            {
                User = sender,
                Friend = receiver,
                Status = "PENDING",
                CreatedAt = DateTime.Now
            };
            await _fsRepo.SaveAsync(friendship);
            UserResponse userResponse = _mapper.Map<UserResponse>(sender);
            await _hubContext.Clients.User(fromId).SendAsync("SendFriendRequestStatus", true);
            await _hubContext.Clients.User(toId).SendAsync("SendFriendRequest", userResponse);   // gửi thông tin người gửi lời kết bạn đến cho người nhận
        }

        public async Task AcceptFriendRequestAsync(string fromId, string toId)
        {
            User? sender = await _userRepo.FindByIdAsync(fromId);
            if (sender == null)
            {
                _logger.LogWarning("AcceptFriendRequest Failed: Sender not found");
                return;
            }
            User? receiver = await _userRepo.FindByIdAsync(toId);
            if (receiver == null)
            {
                _logger.LogWarning("AcceptFriendRequest Failed: Receiver not found");
                return;
            }
            Friendship? friendship = await _fsRepo.FindByUserAndFriendAsync(sender, receiver);
            if (friendship == null)
            {
                _logger.LogWarning("AcceptFriendRequest Failed: Friend request not found");
                return;
            }
            if(friendship.Status == "FRIEND")
            {
                _logger.LogWarning("AccpetFriendRequest Failed: Both sender and receiver are friend");
                return;
            }
            await _fsRepo.UpdateStatus(friendship, "FRIEND");
            await _hubContext.Clients.User(fromId).SendAsync("AcceptFriendRequest", receiver);
            await _hubContext.Clients.User(toId).SendAsync("AcceptFriendRequestStatus", true);
        }
    }
}