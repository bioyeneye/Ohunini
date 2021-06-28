using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ohunini.DataAccess.Entities.Enums;

namespace Ohunini.Business.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OhuniniAuthUser : ClaimsPrincipal
    {
        public OhuniniAuthUser(ClaimsPrincipal principal) : base(principal)
        {
            
        }

        public string GetClaimValue(string key)
        {
            if (!(this.Identity is ClaimsIdentity))
            {
                return null;
            }

            var claim = this.Claims.FirstOrDefault(c => c.Type == key);
            return claim?.Value;
        }
        
        public string TenantId
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity))
                {
                    return null;
                }

                var claim = this.Claims.FirstOrDefault(c => c.Type == OhuniniClaims.Tenant);
                return claim?.Value;
            }
        }
        
        public string HouseId
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity))
                {
                    return null;
                }

                var claim = this.Claims.FirstOrDefault(c => c.Type == OhuniniClaims.House);
                return claim?.Value;
            }
        }
        
        public UserTypes UserType
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity))
                {
                    return default;
                }

                var claim = this.Claims.FirstOrDefault(c => c.Type == OhuniniClaims.UserType);
                return claim != null ? (UserTypes) int.Parse(claim.Value) : default;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OhuniniClaims
    {
        public const string Tenant = "tenant";
        public const string House = "house";
        public const string UserType = "usertype";
    }
}
