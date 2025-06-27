using System.Text.Json.Serialization;

namespace ChatAppApi.Dtos
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        public static ApiResponse<T> CreateSuccess(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> CreateFail(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default
            };
        }
    }
}
