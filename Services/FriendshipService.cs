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
        private readonly NotificationRepository _notifRepo;

        public FriendshipService(ILogger<FriendshipService> logger, IMapper mapper, IHubContext<ApplicationHub> hubContext, FriendshipRepository fsRepo, UserRepository userRepo, NotificationRepository notifRepo)
        {
            _logger = logger;
            _mapper = mapper;
            _hubContext = hubContext;
            _fsRepo = fsRepo;
            _userRepo = userRepo;
            _notifRepo = notifRepo;
        }

        public async Task<ApiResponse<List<FriendRequestResponse>>> GetFriendRequestsAsync(string userId)
        {
            User user = await _userRepo.FindByIdAsync(userId) ?? throw new AppException(ErrorCode.UserNotFound);
            List<Friendship> friendships = await _fsRepo.FindFriendRequestByUserAsync(user);
            List<FriendRequestResponse> responses = _mapper.Map<List<FriendRequestResponse>>(friendships);
            return ApiResponse<List<FriendRequestResponse>>.CreateSuccess(responses);
        }

        public async Task<ApiResponse<List<FriendResponse>>> GetUserFriends(string userId)
        {
            User user = await _userRepo.FindByIdAsync(userId) ?? throw new AppException(ErrorCode.UserNotFound);
            List<Friendship> friendships = await _fsRepo.FindByUserAsync(user);
            List<FriendResponse> friendResponses = new();
            foreach(Friendship friendship in friendships)
            {
                FriendResponse friendResponse = new();
                if(friendship.User.Id.ToString() == userId) // xác định xem user là người gửi hay người nhận request để lấy ra friend cho đúng
                {
                    friendResponse.Id = friendship.Friend.Id;
                    friendResponse.Username = friendship.Friend.Username;
                } else 
                {
                    friendResponse.Id = friendship.User.Id;
                    friendResponse.Username = friendship.User.Username;
                }
                friendResponses.Add(friendResponse);
            }
            return ApiResponse<List<FriendResponse>>.CreateSuccess(friendResponses);
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
            // push notification
            Notification notification = new()
            {
                User = sender,
                Content = "<p><b>" + receiver.Username + "</b> has accepted your friend request</p>",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            notification = await _notifRepo.SaveAsync(notification);
            NotificationResponse notificationResponse = _mapper.Map<NotificationResponse>(notification);
            // send hub message
            await _hubContext.Clients.User(fromId).SendAsync("AcceptFriendRequest", notificationResponse);
            await _hubContext.Clients.User(toId).SendAsync("AcceptFriendRequestStatus", fromId);
        }

        public async Task DeclineFriendRequestAsync(string fromId, string toId)
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
            // send hub message
            await _hubContext.Clients.User(toId).SendAsync("DeclineFriendRequestStatus", fromId);
        }
    }
}