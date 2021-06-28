using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ohunini.Business.Models;

namespace Ohunini.Api.Controllers
{
    /// <inheritdoc />
    public class BaseController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        /// <inheritdoc />
        public BaseController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        /// <summary>
        /// 
        /// </summary>
        public OhuniniAuthUser CurrentUser
        {
            get
            {
                return new OhuniniAuthUser(User as ClaimsPrincipal);
            }
        }

        internal IActionResult HandleException(Exception ex, ILogger logger)
        {
            logger.LogError(ex, ex.Message);
            if (_environment != null && (_environment.IsDevelopment() || _environment.IsEnvironment("uat")))
            {
                return BadRequest(Business.Models.Response.Failed(ex.Message));
            }

            return BadRequest(Business.Models.Response.Failed("Something went wrong. It's not you, it's us. Please give it another try."));
        }
    }
}