namespace ChatAppApi.Exceptions
{
    public class ErrorCode
    {
        public static readonly ErrorCode UserNotFound = new("User not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode RoleNotFound = new("Role not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode UserAlreadyExists = new("User already exists", StatusCodes.Status409Conflict);
        public static readonly ErrorCode InvalidToken = new("Invalid token", StatusCodes.Status401Unauthorized);
        public static readonly ErrorCode PasswordNotMatch = new("Password do not match", StatusCodes.Status400BadRequest);
        public static readonly ErrorCode WrongPassword = new("Wrong password", StatusCodes.Status401Unauthorized);

        public string Message { get; set; } = default!;
        public int StatusCode { get; set; }
        public ErrorCode(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}