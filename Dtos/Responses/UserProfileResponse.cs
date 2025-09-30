namespace ChatAppApi.Dtos.Responses
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public bool IsFemale { get; set; }
        public DateTime BirthDate { get; set; }
    }
}