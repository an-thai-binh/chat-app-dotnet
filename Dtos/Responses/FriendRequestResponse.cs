using ChatAppApi.Models;

namespace ChatAppApi.Dtos.Responses
{
    public class FriendRequestResponse
    {
        public long Id { get; set; }
        public UserResponse User { get; set; } = default!;
        public UserResponse Friend { get; set; } = default!;
        public string Status { get; set; } = default!; // PENDING, FRIEND
        public DateTime CreatedAt { get; set; }
    }
}
