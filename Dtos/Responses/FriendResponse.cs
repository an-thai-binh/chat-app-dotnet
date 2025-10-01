using ChatAppApi.Models;

namespace ChatAppApi.Dtos.Responses
{
    public class FriendResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = default!;
    }
}