namespace ChatAppApi.Dtos.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
