namespace ApiGympass.Middleware
{
    public class UnauthorizedErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public UnauthorizedErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized Access");
                return;
            }
            
            await _next(context);
        }
    }
}