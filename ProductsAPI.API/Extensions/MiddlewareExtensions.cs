using ProductsAPI.API.Middleware;
using System.Runtime.CompilerServices;

namespace ProductsAPI.API.Extensions
{
    public static class MiddlewareExtensions
    {
      public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }

    }
}
