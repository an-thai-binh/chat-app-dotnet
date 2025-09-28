using AutoMapper;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Models;

namespace ChatAppApi.Profiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id));
        }
    }
}
