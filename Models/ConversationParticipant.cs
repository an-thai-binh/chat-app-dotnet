namespace ChatAppApi.Models
{
    public class ConversationParticipant
    {
        public long Id { get; set; }
        public Conversation Conversation { get; set; } = default!;
        public User User { get; set; } = default!;
        public DateTime JoinedAt { get; set; }
    }
}
