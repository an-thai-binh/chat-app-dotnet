using AutoMapper;
using ChatAppApi.Dtos;
using ChatAppApi.Dtos.Requests;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Exceptions;
using ChatAppApi.Models;
using ChatAppApi.Repositories;
using ChatAppApi.Utils;

namespace ChatAppApi.Services
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Transactional _transactional;
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepo;
        private readonly RoleRepository _roleRepo;
        private readonly FriendshipRepository _fsRepo;

        public UserService(IHttpContextAccessor httpContextAccessor, Transactional transactional, IMapper mapper, UserRepository userRepo, RoleRepository roleRepo, FriendshipRepository fsRepo)
        {
            _httpContextAccessor = httpContextAccessor;
            _transactional = transactional;
            _mapper = mapper;
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _fsRepo = fsRepo;
        }

        public async Task<ApiResponse<Page<UserResponse>>> IndexAsync(Pageable pageable)
        {
            Page<User> users = await _userRepo.FindAllAsync(pageable);
            Page<UserResponse> userResponses = new Page<UserResponse>
            {
                Contents = _mapper.Map<List<UserResponse>>(users.Contents),
                PageNumber = users.PageNumber,
                PageSize = users.PageSize,
                TotalItems = users.TotalItems
            };
            return ApiResponse<Page<UserResponse>>.CreateSuccess(userResponses);
        }

        public async Task<ApiResponse<UserResponse>> ShowAsync(string id)
        {
            User user = await _userRepo.FindByIdAsync(id) ?? throw new AppException(ErrorCode.UserNotFound);
            UserResponse userResponse = _mapper.Map<UserResponse>(user);
            return ApiResponse<UserResponse>.CreateSuccess(userResponse);
        }

        public async Task<ApiResponse<UserProfileResponse>> ShowProfileAsync(string id)
        {
            User user = await _userRepo.FindByIdAsync(id) ?? throw new AppException(ErrorCode.UserNotFound);
            UserProfileResponse userProfileResponse = _mapper.Map<UserProfileResponse>(user);
            return ApiResponse<UserProfileResponse>.CreateSuccess(userProfileResponse);
        }

        public async Task<ApiResponse<UserFriendSearchResponse>> SearchUserInFriendAsync(string userId, string query)
        {
            User user = await _userRepo.FindByIdAsync(userId) ?? throw new AppException(ErrorCode.UserNotFound);
            User foundUser = await _userRepo.FindByIdentifierAsync(query) ?? throw new AppException(ErrorCode.UserNotFound);
            Friendship? friendship = await _fsRepo.FindByUserAndFriendAsync(user, foundUser);
            String friendStatus = "NONE";
            Boolean isSender = false;
            if(friendship != null)
            {
                friendStatus = friendship.Status;
                if(friendship.User.Id.ToString() == userId)
                {
                    isSender = true;
                }
            }
            UserFriendSearchResponse response = new UserFriendSearchResponse
            {
                Id = foundUser.Id,
                Username = foundUser.Username,
                FriendStatus = friendStatus,
                IsSender = isSender
            };
            return ApiResponse<UserFriendSearchResponse>.CreateSuccess(response);
        }

        public async Task<ApiResponse<UserResponse>> CreateAsync(UserCreationRequest request)
        {
            User? createdUser = null;
            await _transactional.RunAsync(async () =>
            {
                if (await _userRepo.ExistsByUsernameAsync(request.Username) || await _userRepo.ExistsByEmailAsync(request.Email))
                {
                    throw new AppException(ErrorCode.UserAlreadyExists);
                }
                // check match password
                if(request.ConfirmPassword != request.Password)
                {
                    throw new AppException(ErrorCode.PasswordNotMatch);
                }
                // hash password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                var user = _mapper.Map<User>(request);
                user.Password = hashedPassword;
                // default display name
                user.DisplayName = request.Username;
                // add role
                Role role = await _roleRepo.FindByNameAsync("ROLE_USER") ?? throw new AppException(ErrorCode.RoleNotFound);
                user.Roles.Add(role);
                // save
                createdUser = await _userRepo.SaveAsync(user);
            });
            UserResponse userResponse = _mapper.Map<UserResponse>(createdUser);
            return ApiResponse<UserResponse>.CreateSuccess(userResponse);
        }
    }
}
