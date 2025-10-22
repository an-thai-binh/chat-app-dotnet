using System.Text.Json.Serialization;

namespace ChatAppApi.Models
{
    public class Conversation
    {
        public long Id { get; set; }
        public bool IsGroup { get; set; }
        public string GroupName { get; set; } = default!;
        public User GroupCreator { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public List<ConversationParticipant> Participants { get; set; } = new();
        [JsonIgnore]
        public List<Message> Messages { get; set; } = new();
        [JsonIgnore]
        public List<UserConversation> UserConversations { get; set; } = new();

    }
}
