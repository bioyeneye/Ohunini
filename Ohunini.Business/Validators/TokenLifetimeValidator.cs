using System;
using Microsoft.IdentityModel.Tokens;

namespace Ohunini.Business.Validators
{
    /// <summary>
    /// Token life validator
    /// </summary>
    public static class TokenLifetimeValidator
    {
        /// <summary>
        /// Validate the token using date not before, expires
        /// </summary>
        /// <param name="notBefore"></param>
        /// <param name="expires"></param>
        /// <param name="tokenToValidate"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool Validate(
            DateTime? notBefore,
            DateTime? expires,
            SecurityToken tokenToValidate,
            TokenValidationParameters @param
        )
        {
            return (expires != null && expires > DateTime.UtcNow);
        }
    }
}