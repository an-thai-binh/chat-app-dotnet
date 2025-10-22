

using System.Text.Json.Serialization;

namespace ChatAppApi.Models
{
    public class Message
    {
        public long Id { get; set; }
        public Conversation Conversation { get; set; } = default!;
        public User User { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string Type { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public List<MessageSeen> MessageSeens { get; set; } = new();
        [JsonIgnore]
        public List<UserConversation> UserConversations { get; set; } = new();
    }
}
