using demo_api.Attributes;
using System.Text;
using System.Text.Json;

namespace demo_api.Middlewares
{
    public class ResponseWrapperApi
    {
        private readonly RequestDelegate _next; 
        public ResponseWrapperApi(RequestDelegate next) { _next = next; }
        public async Task Invoke(HttpContext context)
        {
            var originalBody = context.Response.Body;
            using var newBody = new MemoryStream();
            context.Response.Body = newBody;

            await _next(context); 

            newBody.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(newBody).ReadToEndAsync();

            object responseData;
            try
            {
                responseData = JsonSerializer.Deserialize<object>(responseBody);
            }
            catch
            {
                responseData = responseBody;
            }

            var endpoint = context.GetEndpoint();
            var messageAttr = endpoint?.Metadata.GetMetadata<MessageAttribute>();
            var message = messageAttr?.Text ?? "";
                         

            var wrappedResponse = new
            {
                code = context.Response.StatusCode,
                message,
                data = responseData
            };

            context.Response.Body = originalBody;
            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(wrappedResponse);
            await context.Response.WriteAsync(json, Encoding.UTF8);
        }

    }
}
