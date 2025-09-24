namespace ChatAppApi.Models
{
    public class Friendship
    {
        public long Id { get; set; }
        public User User { get; set; } = default!;
        public User Friend { get; set; } = default!;
        public string Status { get; set; } = default!; // PENDING, FRIEND
        public DateTime CreatedAt { get; set; }
    }
}