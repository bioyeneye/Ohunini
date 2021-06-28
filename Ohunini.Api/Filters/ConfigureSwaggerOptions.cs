using System;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Ohunini.Business.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ohunini.Api.Filters
{
	/// <inheritdoc />
	public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
	{
		private readonly IApiVersionDescriptionProvider _provider;
		private readonly IConfiguration _configuration;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="configuration"></param>
		public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
		{
			_provider = provider;
			_configuration = configuration;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		public void Configure(SwaggerGenOptions options)
		{
			var swaggerConfiguration = _configuration.GetSection(nameof(SwaggerConfiguration)).Get<SwaggerConfiguration>();

			if (!swaggerConfiguration.IsEnabled) return;
			foreach (var description in _provider.ApiVersionDescriptions)
			{
				options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description, swaggerConfiguration.Name));
			}
		}

		private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description, string apiName)
		{
			var info = new OpenApiInfo()
			{
				Title = $"{apiName} {description.GroupName}",
				Version = $"v{description.ApiVersion.ToString()}",
				Description = apiName,
				License = new OpenApiLicense
				{
					Name = "Microsoft Licence",
					Url = new Uri("https://ohunini.net/licence"),
				},
				Contact = new OpenApiContact
				{
					Name = "Oyeneye Bolaji",
					Email = "simisola.oyeneye@gmail.com",
					Url = new Uri("https://twitter.com/thecoderefiner"),
				},
			};

			if (description.IsDeprecated)
			{
				info.Description += " This API version has been deprecated.";
			}

			return info;
		}
	}
}

