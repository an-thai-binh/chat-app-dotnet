using System.Text.Json.Serialization;

namespace ChatAppApi.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public bool IsFemale { get; set; }
        public DateTime BirthDate { get; set; }
        public List<Role> Roles { get; set; } = new();
        [JsonIgnore]
        public List<Friendship> FriendSent { get; set; } = new();
        [JsonIgnore]
        public List<Friendship> FriendReceived { get; set; } = new();
        [JsonIgnore]
        public List<Notification> Notifications { get; set; } = new();
    } 
}