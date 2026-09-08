using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Platform.API.Responses;
using Platform.Core.Services;
using System.Text.Json;

namespace Platform.API.Exceptions
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly ILocalizationService _localizationService;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            ILocalizationService localizationService)
        {
            _logger = logger;
            _localizationService = localizationService;
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
                case ValidationException ex:
                    {
                        var errors = ex.Errors.Select(x => _localizationService.Get(x.ErrorMessage)).ToList();
                        response = Result<object>.Failure(errors);
                        break;
                    }


                case ApplicationException ex:

                    response = Result<object>.Failure(_localizationService.Get(ex.Message));
                    break;

                default:
                    response = Result<object>.Failure(_localizationService.Get("UnexpectedError"));
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