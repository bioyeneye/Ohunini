using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Ohunini.Api.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ApiVersionRoutePrefixConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _versionConstraintTemplate;

        internal ApiVersionRoutePrefixConvention() => this._versionConstraintTemplate = new AttributeRouteModel
        {
            Template = "v{version:apiVersion}",
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        public void Apply(ApplicationModel application)
        {
            foreach (var selector in application.Controllers
                .SelectMany(controller => controller.Selectors)
                .Where(selector => selector.AttributeRouteModel != default))
            {
                selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(this._versionConstraintTemplate, selector.AttributeRouteModel);
            }
        }
    }
}