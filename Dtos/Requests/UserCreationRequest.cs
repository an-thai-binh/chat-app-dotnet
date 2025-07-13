namespace ChatAppApi.Dtos.Requests
{
    public class UserCreationRequest
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public string Email { get; set; } = default!;
        public bool IsFemale { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
