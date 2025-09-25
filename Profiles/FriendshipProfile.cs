using AutoMapper;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Models;

namespace ChatAppApi.Profiles
{
    public class FriendshipProfile : Profile
    {
        public FriendshipProfile()
        {
            CreateMap<Friendship, FriendRequestResponse>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Friend, opt => opt.MapFrom(src => src.Friend));
        }
    }
}
