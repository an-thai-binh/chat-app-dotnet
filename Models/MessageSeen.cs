
namespace ChatAppApi.Models
{
    public class MessageSeen
    {
        public long Id { get; set; }
        public Message Message { get; set; } = default!;
        public User User { get; set; } = default!;
        public DateTime SeenAt { get; set; }
    }
}
