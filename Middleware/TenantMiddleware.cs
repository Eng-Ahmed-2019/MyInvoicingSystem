namespace InvoicingSystem.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Guid companyId = Guid.Empty;

            if (context.Request.Headers.TryGetValue("X-Company-Id", out var headerValue))
            {
                Guid.TryParse(headerValue, out companyId);
            }

            if (companyId == Guid.Empty && context.User?.Identity?.IsAuthenticated == true)
            {
                var claimValue = context.User.FindFirst("CompanyId")?.Value;
                Guid.TryParse(claimValue, out companyId);
            }

            if (companyId != Guid.Empty)
            {
                context.Items["CompanyId"] = companyId;
            }

            await _next(context);
        }
    }

    public static class TenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}