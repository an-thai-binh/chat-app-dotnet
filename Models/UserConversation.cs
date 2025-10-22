
namespace ChatAppApi.Models
{
    public class UserConversation
    {
        public long Id { get; set; }
        public User User { get; set; } = default!;
        public Conversation Conversation { get; set; } = default!;
        public Message LatestMessage { get; set; } = default!;
        public DateTime LatestMessageTime { get; set; }
        public int UnreadCount { get; set; }
        public bool IsPinned { get; set; }

    }
}
