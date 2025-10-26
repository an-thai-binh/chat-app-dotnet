using AutoMapper;
using ChatAppApi.Dtos.Responses;
using ChatAppApi.Models;

namespace ChatAppApi.Profiles
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            CreateMap<Conversation, ConversationResponse>();
        }
    }
}
