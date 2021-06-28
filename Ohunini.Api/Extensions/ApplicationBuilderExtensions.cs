using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Ohunini.Api.Middlewares;
using Ohunini.Business.Models;

namespace Ohunini.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Helper for configuring security header
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConfigureSecurityHeaders(this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts(options => options.MaxAge(365).IncludeSubdomains());
            }

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseHttpsRedirection();
            app.UseMiddleware<SecurityHeadersMiddleware>();

            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(options => options.NoReferrer());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXfo(options => options.Deny());

            app.Use((context, next) =>
            {
                if (context.Request.IsHttps)
                    context.Response.Headers.Append("Expect-CT",
                        "max-age=0; report-uri=\"https://fuddora.net/report-ct\"");
                return next.Invoke();
            });

            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };

            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardOptions);

            return app;
        }

        /// <summary>
        /// Middleware for swagger, call this to add custom swagger to the middleware pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <param name="provider"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IApplicationBuilder ApplicationSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider, IConfiguration configuration)
        {
            var swaggerConfiguration = configuration.GetSection(nameof(SwaggerConfiguration)).Get<SwaggerConfiguration>();

            if (!swaggerConfiguration.IsEnabled) return app;
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    options.DisplayRequestDuration();
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"{swaggerConfiguration.Name} - v{description.GroupName.ToUpperInvariant()}");
                    }
                });

            return app;
        }
    }
}