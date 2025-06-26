namespace ChatAppApi.Dtos.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = default!;
        public string? DisplayName { get; set; }
        public bool IsFemale { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
