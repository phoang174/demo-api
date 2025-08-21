using Azure.Core;
using demo_api.Exceptions;
using Domain.IRepository;
using System.Security.Claims;

namespace demo_api.Middlewares
{
    public class CheckBlackListMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckBlackListMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IBlackListRepository blackList)
        {

            var endpoint = context.GetEndpoint();
            var hasAuthorize = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null;
            if (hasAuthorize)
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    Console.WriteLine("Empty");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized: Bearer token missing");
                    return; 
                }
                var accessToken = authHeader.Substring("Bearer ".Length).Trim();

                var result = await blackList.CheckTokenExist(accessToken);
                if (result == true)
                {
                    throw new CustomException("User has been logged out", 400);
                }
            }

            await _next(context);

        }
    }
}
