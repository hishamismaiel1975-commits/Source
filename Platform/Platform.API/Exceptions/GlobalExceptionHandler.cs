using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Platform.API.Responses;
using System.Globalization;
using System.Text.Json;

namespace Platform.API.Exceptions
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, exception.Message);
            Result<object> response;
            switch (exception)
            {
                case ApplicationException ex:
                    response = Result<object>.Failure(ex.Message);
                    break;

                default:
                    response = Result<object>.Failure(CultureInfo.CurrentUICulture.Name == "en" ? "An unexpected error occurred." : "حدث خطأ غير متوقع.");
                    break;
            }

            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(response),
                cancellationToken);

            return true;
        }
    }

}