using System;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fudoora.Common.Filters
{
    /// <summary>
    /// Enum filter for swagger
    /// </summary>
    public class EnumSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Caller action for the enum filter
        /// </summary>
        /// <param name="model"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            if (!context.Type.IsEnum) return;
            model.Enum.Clear();
            Enum.GetNames(context.Type)
                .ToList()
                .ForEach(name => model.Enum.Add(new OpenApiString($"{Convert.ToInt64(Enum.Parse(context.Type, name))} - {name}")));
        }
    }
}