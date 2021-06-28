using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ohunini.Business.Models;

namespace Ohunini.Api.Filters
{
    /// <inheritdoc />
    public class ValidateModelStateActionFilter : IAsyncActionFilter
    {
        /// <inheritdoc />
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Check if ModelState is valid.
            var validationErrors = new List<ValidationError>();
            if (!context.ModelState.IsValid)
            {
                var validationError = context?.ModelState?
                    .Keys
                    .Where(i => context.ModelState[i].Errors.Count > 0)?
                    .Select(k => context.ModelState[k]?.Errors?.First()?.ErrorMessage)?
                    .ToList();


                new SerializableError(context.ModelState).ToList().ForEach(x => validationErrors.Add(new ValidationError { InpField = x.Key, ErrMessage = x.Value }));

                var result = new BadRequestObjectResult(Response.ValidationError(validationErrors, validationError));
                result.ContentTypes.Add(MediaTypeNames.Application.Json);
                result.ContentTypes.Add(MediaTypeNames.Application.Xml);
                context.Result = result;
            }
            else
            {
                await next();
            }
        }
    }
}