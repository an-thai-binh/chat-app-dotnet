using AutoMapper;
using ChatAppApi.Dtos.Requests;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Models;

namespace ChatAppApi.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserCreationRequest, User>();
            CreateMap<User, UserResponse>();
        }
    }
}
