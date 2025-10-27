using AutoMapper;
using ChatAppApi.Dtos;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Exceptions;
using ChatAppApi.Models;
using ChatAppApi.Repositories;
using ChatAppApi.Utils;
using System.Text.Json;

namespace ChatAppApi.Services
{
    public class ConversationService
    {
        private readonly IMapper _mapper;
        private readonly Transactional _transactional;
        private readonly UserRepository _userRepo;
        private readonly FriendshipRepository _fsRepo;
        private readonly ConversationRepository _converRepo;

        public ConversationService(IMapper mapper, Transactional transactional, UserRepository userRepo, FriendshipRepository fsRepo, ConversationRepository converRepo)
        {
            _mapper = mapper;
            _transactional = transactional;
            _userRepo = userRepo;
            _fsRepo = fsRepo;
            _converRepo = converRepo;
        }

        public async Task<ApiResponse<ConversationResponse>> ShowByUserAndFriendAsync(string userId, string friendId)
        {
            User user = await _userRepo.FindByIdAsync(userId) ?? throw new AppException(ErrorCode.UserNotFound);
            User friend = await _userRepo.FindByIdAsync(friendId) ?? throw new AppException(ErrorCode.UserNotFound);
            Friendship? friendship = await _fsRepo.FindByUserAndFriendWithConversationAsync(user, friend);
            if (friendship == null || friendship.Status != "FRIEND")
            {
                throw new AppException(ErrorCode.FriendshipNotFound);
            }
            Conversation? conversation = friendship.PrivateConversation;
            if (conversation == null)
            {
                await _transactional.RunAsync(async () =>
                {
                    var groupName = new
                    {
                        GroupName = "",
                        Id1 = userId,
                        Name1 = user.Username,
                        Id2 = friendId,
                        Name2 = friend.Username
                    };
                    conversation = new Conversation
                    {
                        IsGroup = false,
                        GroupName = JsonSerializer.Serialize(groupName),
                        CreatedAt = DateTime.Now
                    };
                    conversation = await _converRepo.SaveAsync(conversation);
                    friendship.PrivateConversation = conversation;
                    await _fsRepo.UpdateAsync(friendship);
                });
            }
            ConversationResponse conversationResponse = _mapper.Map<ConversationResponse>(conversation);
            conversationResponse.GroupName = GetNameFromGroupNameJson(conversation?.GroupName ?? "", userId);
            return ApiResponse<ConversationResponse>.CreateSuccess(conversationResponse);
        }

        /// <summary>
        /// Lấy ra tên của conversation thông qua chuỗi json từ GroupName
        /// </summary>
        /// <param name="groupNameJson">chuỗi json GroupName</param>
        /// <param name="userId">id người lấy ra</param>
        /// <returns>tên của conversation</returns>
        private string GetNameFromGroupNameJson(string groupNameJson, string userId) {
            Dictionary<string, string>? groupNameDict = JsonSerializer.Deserialize<Dictionary<string, string>>(groupNameJson);
            if(groupNameDict == null || groupNameDict.Count == 0)
            {
                return "";
            }
            if (!string.IsNullOrEmpty(groupNameDict["GroupName"]))
            {
                return groupNameDict["GroupName"];
            }
            if (groupNameDict["Id1"] == userId)
            {
                return groupNameDict["Name2"];
            } else
            {
                return groupNameDict["Name1"];
            }
        }
    }
}