using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ohunini.Api.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1")]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserProfileController : BaseController
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UserProfileController> _logger;

        /// <inheritdoc />
        public UserProfileController(IWebHostEnvironment environment, ILogger<UserProfileController> logger) : base(environment)
        {
            _environment = environment;
            _logger = logger;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetUserProfileById(string id)
        {
            try
            {
                return Ok(new
                {

                });
            }
            catch (Exception e)
            {
                return HandleException(e, _logger);
            }
        }
    }
}