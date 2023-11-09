using System.Text;
using ApiGympass.Utils;
using Newtonsoft.Json;

namespace ApiGympass.Middlewares
{
    public class ApiResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiResponseMiddleware> _logger;

        public ApiResponseMiddleware(RequestDelegate next, ILogger<ApiResponseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using (var responseBodyStream = new MemoryStream())
            {
                context.Response.Body = responseBodyStream;

                await _next(context);

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                var body = await new StreamReader(responseBodyStream).ReadToEndAsync();

                string responseContent;
                if (!string.IsNullOrEmpty(body))
                {
                    if (context.Response.StatusCode >= 400)
                    {
                        var errorResponse = new ApiResponse<string>(null, body, true);
                        responseContent = JsonConvert.SerializeObject(errorResponse);
                    }
                    else
                    {
                        var successResponse = new ApiResponse<object>(body, "OK", false);
                        responseContent = JsonConvert.SerializeObject(successResponse);
                    }
                }
                else
                {
                    responseContent = body;
                }

                var responseBytes = Encoding.UTF8.GetBytes(responseContent);

                // Atualiza o cabeçalho Content-Length
                context.Response.ContentLength = responseBytes.Length;

                // Restaura o stream original e escreve a resposta modificada
                context.Response.Body = originalBodyStream;
                await context.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
        }
    }
}
