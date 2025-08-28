using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FluentValidation;
using FluentValidation.Results;

namespace demo_api.Exceptions
{
    internal sealed class ValidationExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ValidationExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;

        public ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger, IProblemDetailsService problemDetailsService)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is ValidationException validationEx)
            {
                _logger.LogWarning("Validation failed: {errors} at {time}",
                    validationEx.Errors, DateTime.UtcNow);

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                httpContext.Response.ContentType = "application/json";

                var errors = validationEx.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage,
                    Severity = e.Severity.ToString()
                }).ToArray();

                var context = new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    Exception = exception,
                    ProblemDetails = new ProblemDetails
                    {
                        Title = "Validation failed",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = null, 
                        Extensions = { ["errors"] = errors }
                    }
                };

                await _problemDetailsService.TryWriteAsync(context);
                return true;
            }

            return false;
        }
    }
}
