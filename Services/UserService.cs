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
        private readonly Transactional _transactional;
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepo;
        private readonly RoleRepository _roleRepo;

        public UserService(Transactional transactional, IMapper mapper, UserRepository userRepo, RoleRepository roleRepo)
        {
            _transactional = transactional;
            _mapper = mapper;
            _userRepo = userRepo;
            _roleRepo = roleRepo;
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

        public async Task<ApiResponse<UserResponse>> CreateAsync(UserCreationRequest request)
        {
            User? createdUser = null;
            await _transactional.RunAsync(async () =>
            {
                if (await _userRepo.ExistsByUsernameAsync(request.Username) || await _userRepo.ExistsByEmailAsync(request.Email))
                {
                    throw new AppException(ErrorCode.UserAlreadyExists);
                }
                // hash password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                var user = _mapper.Map<User>(request);
                user.Password = hashedPassword;
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
