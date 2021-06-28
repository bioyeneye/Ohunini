using Fudoora.Common.Filters;
using Microsoft.AspNetCore.Mvc;
using Ohunini.Api.Filters;

namespace Ohunini.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class MvcOptionsExtensions
    {
        internal static MvcOptions AddApiVersionRoutePrefixConvention(this MvcOptions @this)
        {
            @this
                .Conventions
                .Add(new ApiVersionRoutePrefixConvention());

            return @this;
        }
    }
}