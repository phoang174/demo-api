using Application.Service;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using Domain.IRepository;
namespace demo_api.Middlewares
{
    public class GetRolesMiddleware
    {
        private readonly RequestDelegate _next;

        public GetRolesMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context,IUserRepository user)
        {

          

            var endpoint = context.GetEndpoint();
            var hasAuthorize = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null;
            if (hasAuthorize) {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var identity = context.User.Identity as ClaimsIdentity;
                if (userId != null)
                {
                    var param = Convert.ToInt32(userId);
                    var roles = await user.GetUserRolesAsync(param);

                    Console.WriteLine($"User sub: {userId}");
                    foreach (var roleName in roles)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                    }

                    Console.WriteLine("Roles");
                    Console.WriteLine(string.Join(", ", context.User.Claims.Where(e => e.Type == ClaimTypes.Role).Select(c => c.Value)));
                }
            }
   

            await _next(context);
        }

    }
}
