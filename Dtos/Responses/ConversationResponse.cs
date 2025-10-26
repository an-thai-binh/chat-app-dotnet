using ChatAppApi.Models;

namespace ChatAppApi.Dtos.Responses
{
    public class ConversationResponse
    {
        public long Id { get; set; }
        public bool IsGroup { get; set; }
        public string GroupName { get; set; } = default!;
        public User GroupCreator { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
