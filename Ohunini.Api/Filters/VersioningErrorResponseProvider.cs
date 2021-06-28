using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Ohunini.Business.Models;

namespace Ohunini.Api.Filters
{
    /// <inheritdoc />
    public sealed class VersioningErrorResponseProvider: DefaultErrorResponseProvider
    {
        /// <inheritdoc />
        public override IActionResult CreateResponse(ErrorResponseContext context)
        {
            return new BadRequestObjectResult(Response.Failed(context.Message));
        }
    }
}