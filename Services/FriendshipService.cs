using AutoMapper;
using ChatAppApi.Dtos;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Exceptions;
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

        public async Task<ApiResponse<List<FriendRequestResponse>>> GetFriendRequestsAsync(string userId)
        {
            User user = await _userRepo.FindByIdAsync(userId) ?? throw new AppException(ErrorCode.UserNotFound);
            List<Friendship> friendships = await _fsRepo.FindFriendRequestByUser(user);
            List<FriendRequestResponse> responses = _mapper.Map<List<FriendRequestResponse>>(friendships);
            return ApiResponse<List<FriendRequestResponse>>.CreateSuccess(responses);
        } 

        public async Task SendFriendRequestAsync(string fromId, string toId)
        {
            if (fromId == toId) throw new AppException(ErrorCode.SelfActionNotAllowed);
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
            friendship = await _fsRepo.SaveAsync(friendship);
            await _hubContext.Clients.User(fromId).SendAsync("SendFriendRequestStatus", true);
            await _hubContext.Clients.User(toId).SendAsync("SendFriendRequest", friendship);   // gửi thông tin người gửi lời kết bạn đến cho người nhận
        }

        public async Task CancelFriendRequestAsync(string fromId, string toId)
        {
            if (fromId == toId) throw new AppException(ErrorCode.SelfActionNotAllowed);
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
            await _fsRepo.DeleteByUserAndFriendAsync(sender, receiver);
            await _hubContext.Clients.User(fromId).SendAsync("CancelFriendRequestStatus", toId);
            await _hubContext.Clients.User(toId).SendAsync("CancelFriendRequest", fromId);
        }

        public async Task AcceptFriendRequestAsync(string fromId, string toId)
        {
            if (fromId == toId) throw new AppException(ErrorCode.SelfActionNotAllowed);
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