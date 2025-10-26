
namespace ChatAppApi.Models
{
    public class UserConversation
    {
        public long Id { get; set; }
        public User User { get; set; } = default!;
        public Guid UserId { get; set; }
        public Conversation Conversation { get; set; } = default!;
        public long ConversationId { get; set; }
        public Message LatestMessage { get; set; } = default!;
        public long LatestMessageId { get; set; }
        public DateTime LatestMessageTime { get; set; }
        public int UnreadCount { get; set; }
        public bool IsPinned { get; set; }

    }
}
