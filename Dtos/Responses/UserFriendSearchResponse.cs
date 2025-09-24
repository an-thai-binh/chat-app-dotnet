namespace ChatAppApi.Dtos.Responses
{
    public class UserFriendSearchResponse
    {
        public Guid Id { get; set; }
        public String Username { get; set; } = default!;
        public String FriendStatus { get; set; } = default!;

    }
}
