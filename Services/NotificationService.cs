using AutoMapper;
using ChatAppApi.Dtos;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Exceptions;
using ChatAppApi.Models;
using ChatAppApi.Repositories;

namespace ChatAppApi.Services
{
    public class NotificationService
    {
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepo;
        private readonly NotificationRepository _notifRepo;

        public NotificationService(IMapper mapper, UserRepository userRepo, NotificationRepository notifRepo)
        {
            _mapper = mapper;
            _userRepo = userRepo;
            _notifRepo = notifRepo;
        }

        public async Task<ApiResponse<List<NotificationResponse>>> GetLatestNotificationsAsync(string userId, int quantity)
        {
            if (quantity <= 0) throw new AppException(ErrorCode.InvalidParameters);
            User user = await _userRepo.FindByIdAsync(userId) ?? throw new AppException(ErrorCode.UserNotFound);
            List<Notification> notifications = await _notifRepo.FindLatestByUserAsync(user, quantity);
            List<NotificationResponse> responses = _mapper.Map<List<NotificationResponse>>(notifications);
            return ApiResponse<List<NotificationResponse>>.CreateSuccess(responses);
        }

        public async Task MaskReadForUnreadNotificationsAsync(string userId)
        {
            User user = await _userRepo.FindByIdAsync(userId) ?? throw new AppException(ErrorCode.UserNotFound);
            await _notifRepo.UpdateReadForUnreadNotificationsByUserAsync(user);
        }
    }
}