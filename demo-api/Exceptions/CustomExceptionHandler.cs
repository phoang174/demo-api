using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace demo_api.Exceptions
{
    internal sealed class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger, IProblemDetailsService problemDetailsService) :IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Log lỗi
            logger.LogError("Error Message: {exceptionMessage}, Time of occurrence {time}",
                exception.Message, DateTime.UtcNow);

            if (exception is CustomException customEx)
            {

                // Gắn status code và message vào response
                httpContext.Response.StatusCode = customEx.StatusCode;
                httpContext.Response.ContentType = "application/json";

          
                var context = new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    Exception = exception,
                    ProblemDetails = new ProblemDetails
                    {
                        Detail = customEx.Message,
                        Status = customEx.StatusCode
                    }
                };

                return await problemDetailsService.TryWriteAsync(context);
            }

            return false; 
        }

    }
}
