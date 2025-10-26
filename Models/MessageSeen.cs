
namespace ChatAppApi.Models
{
    public class MessageSeen
    {
        public long Id { get; set; }
        public Message Message { get; set; } = default!;
        public long MessageId { get; set; }
        public User User { get; set; } = default!;
        public Guid UserId { get; set; }
        public DateTime SeenAt { get; set; }
    }
}
