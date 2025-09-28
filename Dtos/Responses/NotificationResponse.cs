using ChatAppApi.Models;

namespace ChatAppApi.Dtos.Responses
{
    public class NotificationResponse
    {
        public long Id { get; set; }
        public string UserId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
