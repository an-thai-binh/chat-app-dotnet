using ChatAppApi.Dtos;
using System.Text.Json;

namespace ChatAppApi.Exceptions
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            } catch(AppException e)
            {
                var errorCode = e.ErrorCode;
                _logger.LogError(e, errorCode.Message);

                HttpResponse response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = errorCode.StatusCode;
                var apiResponse = ApiResponse<object?>.CreateFail(errorCode.Message);
                string responseBody = JsonSerializer.Serialize(apiResponse);

                await response.WriteAsync(responseBody);
            } catch(Exception e)
            {
                _logger.LogError(e, "Unhandled Exception");

                HttpResponse response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = StatusCodes.Status500InternalServerError;
                var apiResponse = ApiResponse<object?>.CreateFail(e.Message);
                string responseBody = JsonSerializer.Serialize(apiResponse);

                await response.WriteAsync(responseBody);
            }
        }
    }
}
