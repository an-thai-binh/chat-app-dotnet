namespace ChatAppApi.Exceptions
{
    public class AppException : Exception
    {
        public ErrorCode ErrorCode { get; set; }

        public AppException(ErrorCode errorCode) : base(errorCode.Message)
        {
            ErrorCode = errorCode;
        }
    }
}
