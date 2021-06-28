using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ohunini.Api.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SecurityHeadersMiddleware
    {
        private const int OneYearInSeconds = 60 * 60 * 24 * 365;
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public SecurityHeadersMiddleware(RequestDelegate next) => this._next = next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers["X-Frame-Options"] = "deny";
            context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            context.Response.Headers["Strict-Transport-Security"] = $"max-age={OneYearInSeconds}; includeSubDomains; preload";
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            //context.Response.Headers["Content-Security-Policy"] = "default-src 'none'";
            //context.Response.Headers["X-Content-Security-Policy"] = "default-src 'none'";
            context.Response.Headers["Server"] = "ohunini.com";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";

            await this._next(context);
        }
    }
}