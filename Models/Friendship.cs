namespace ChatAppApi.Models
{
    public class Friendship
    {
        public long Id { get; set; }
        public User User { get; set; } = default!;
        public Guid UserId { get; set; }
        public User Friend { get; set; } = default!;
        public Guid FriendId { get; set; }
        public string Status { get; set; } = default!; // PENDING, FRIEND
        public DateTime CreatedAt { get; set; }
        public Conversation? PrivateConversation { get; set; }
        public long? PrivateConversationId { get; set; }

    }
}