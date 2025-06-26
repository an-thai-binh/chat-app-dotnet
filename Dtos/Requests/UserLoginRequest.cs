namespace ChatAppApi.Dtos.Requests
{
    public class UserLoginRequest
    {
        public string Identifier { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
