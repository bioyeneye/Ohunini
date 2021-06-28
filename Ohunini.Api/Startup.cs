using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Ohunini.Api.Extensions;

namespace Ohunini.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            IdentityModelEventSource.ShowPII = true;
            services.AddControllers();

            services.AddCors();
            services.AddApplicationService(Configuration);
            services.AddApplicationApiConfiguration();
            services.ConfigureVersioning();
            services.AddSwaggerDoc(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePages();
            }
            
            app.UseConfigureSecurityHeaders(env);
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.ApplicationSwagger(provider, Configuration);

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseWelcomePage();

        }
    }
}