using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fudoora.Common.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ohunini.Api.Filters;
using Ohunini.Business.Configuration.Email;
using Ohunini.Business.Configuration.Email.Implementation;
using Ohunini.Business.Models;
using Ohunini.Business.Validators;
using SendGrid;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ohunini.Api.Extensions
{
    /// <summary>
    /// Common service configuration for service projects
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add email senders - configuration of sendgrid, smtp senders
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddEmailSenders(this IServiceCollection services, IConfiguration configuration)
        {
            var smtpConfiguration = configuration.GetSection(nameof(SmtpConfiguration)).Get<SmtpConfiguration>();
            var sendGridConfiguration = configuration.GetSection(nameof(SendGridConfiguration)).Get<SendGridConfiguration>();

            if (sendGridConfiguration != null && !string.IsNullOrWhiteSpace(sendGridConfiguration.ApiKey))
            {
                services.AddSingleton<ISendGridClient>(_ => new SendGridClient(sendGridConfiguration.ApiKey));
                services.AddSingleton(sendGridConfiguration);
                services.AddTransient<IEmailSender, SendGridEmailSender>();
            }
            else if (smtpConfiguration != null && !string.IsNullOrWhiteSpace(smtpConfiguration.Host))
            {
                services.AddSingleton(smtpConfiguration);
                services.AddTransient<IEmailSender, SmtpEmailSender>();
            }
            else
            {
                services.AddSingleton<IEmailSender, LogEmailSender>();
            }
        }
        
        /// <summary>
        /// Configure versioning
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureVersioning(this IServiceCollection @this)
        {
            return @this
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // https://localhost:44335/v2/weatherforecast
                    options.ErrorResponses = new VersioningErrorResponseProvider();
                    options.ReportApiVersions = true;
                })
                .AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "VVV";
                    options.SubstituteApiVersionInUrl = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                });
        }

        /// <summary>
        /// Configure application services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IServiceCollection AddApplicationService(this IServiceCollection services,
            IConfiguration configuration)
        {
            var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            var swaggerSettings = configuration.GetSection(nameof(SwaggerConfiguration)).Get<SwaggerConfiguration>();
            if (appSettings != null && !string.IsNullOrWhiteSpace(appSettings.Secret))
            {
                services.AddSingleton(appSettings);
            }
            else
            {
                throw new Exception(
                    $"App settings is not set in the configuration file, check the format in {nameof(AppSettings)} class");
            }

            return services;
        }


        /// <summary>
        /// Helps with api configuration like making url lowercase, date formating, model state validation 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationApiConfiguration(this IServiceCollection services)
        {
            //https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-3.1
            //services.AddControllers(options => { options.Filters.Add(typeof(ValidateModelStateActionFilter)); });
            services.AddControllers(options =>
                {
                    // requires using Microsoft.AspNetCore.Mvc.Formatters;
                    options.OutputFormatters.RemoveType<StringOutputFormatter>();
                    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                    options.Filters.Add(typeof(ValidateModelStateActionFilter));
                })
                .AddNewtonsoftJson(options =>
                {
                    // Use the default property (Pascal) casing
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.DateFormatString = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.Culture = CultureInfo.InvariantCulture;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;

                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                 })
                .ConfigureApiBehaviorOptions(options => { });
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });
            return services;
        }

        /// <summary>
        /// Configure swagger for api project
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerDoc(this IServiceCollection services, IConfiguration configuration)
        {
            var swaggerConfiguration = configuration.GetSection(nameof(SwaggerConfiguration)).Get<SwaggerConfiguration>();
            if (swaggerConfiguration != null && !string.IsNullOrWhiteSpace(swaggerConfiguration.Name))
            {
                services.AddSingleton(swaggerConfiguration);
            }
            else
            {
                throw new Exception(
                    $"Swagger settings is not set in the configuration file, check the format in {nameof(SwaggerConfiguration)} class");
            }


            if (!swaggerConfiguration.IsEnabled) return services;
            
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<SwaggerDefaultValues>();
                c.DescribeAllParametersInCamelCase();
                c.OrderActionsBy((apiDesc) =>
                    $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
                
                c.SchemaFilter<EnumSchemaFilter>();
                c.AddSecurityDefinition(appSettings.Scheme, new OpenApiSecurityScheme
                {
                    Description = @$"JWT Authorization header using the Bearer scheme. 
                      {Environment.NewLine}Enter 'Bearer' [space] and then your token in the text input below.
                      {Environment.NewLine}Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = appSettings.Scheme
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = appSettings.Scheme
                            },
                            Scheme = "oauth2",
                            Name = appSettings.Scheme,
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.EnableAnnotations();
                //c.CustomSchemaIds((type)=> type.FullName);
            });
            services.AddSwaggerGenNewtonsoftSupport();
            return services;
        }

        /// <summary>
        /// Service configuration for authentication
        /// </summary>
        /// <param name="services">Service collection(core)</param>
        /// <param name="configuration">Configuration</param>
        /// <param name="serviceLevel">Flag for token signing validation</param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationAuthentication(this IServiceCollection services,
            IConfiguration configuration, bool serviceLevel = false)
        {
            var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            IdentityModelEventSource.ShowPII = true;
            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    sharedOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.RequireHttpsMetadata = false;
                    // options.SaveToken = true;
                    options.Configuration = new OpenIdConnectConfiguration();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = appSettings.ValidateIssuer,
                        ValidateAudience = appSettings.ValidateAudience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = !serviceLevel && appSettings.ValidateIssuerSigningKey,
                        ValidIssuer = appSettings.Issuer,
                        ValidAudience = appSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Secret)),
                        LifetimeValidator = TokenLifetimeValidator.Validate,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }

                            Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                            return Task.CompletedTask;
                        },
                    };
                });

            return services;
        }
    }

    
}