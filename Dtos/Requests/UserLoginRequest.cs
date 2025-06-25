namespace ChatAppApi.Dtos.Requests
{
    public class UserLoginRequest
    {
        public string Identifer { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
