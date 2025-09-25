namespace ChatAppApi.Dtos.Responses
{
    public class UserFriendSearchResponse
    {
        public Guid Id { get; set; }
        public String Username { get; set; } = default!;
        public String FriendStatus { get; set; } = default!;
        public Boolean IsSender { get; set; }   // xác định xem user gửi request có phải là người gửi lời mời kết bạn không
    }
}
