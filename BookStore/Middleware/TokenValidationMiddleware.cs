using Repository.UnitOfwork;

namespace BookStore.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfwork unitOfWork)
        {
            var tokenString = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(tokenString))
            {
                var token = unitOfWork.TokenRepo.Get().FirstOrDefault(t => t.Token1 == tokenString);

                if (token != null)
                {
                    if (token.ExpiryDate <= DateTime.Now)
                    {
                        // Token đã hết hạn, cập nhật trạng thái IsActive
                        token.IsActive = false;
                        unitOfWork.TokenRepo.Update(token);
                        unitOfWork.Save();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token has expired");
                        return;
                    }
                    else if (!token.IsActive)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token is inactive");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
