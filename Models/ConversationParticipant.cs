namespace ChatAppApi.Models
{
    public class ConversationParticipant
    {
        public long Id { get; set; }
        public Conversation Conversation { get; set; } = default!;
        public long ConversationId { get; set; }
        public User User { get; set; } = default!;
        public Guid UserId { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
