namespace ChatAppApi.Models
{
    public class Notification
    {
        public long Id { get; set; }
        public User User { get; set; } = default!;
        public string Content { get; set; } = default!;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
