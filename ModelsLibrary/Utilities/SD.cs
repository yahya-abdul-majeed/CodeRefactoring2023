using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using ModelsLibrary.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace ModelsLibrary.Utilities
{
    public enum GridType
    {
        type1 = 1,
        type2 = 2
    }
    public enum LabStatus
    {
        Occupied,
        Vacant,
        Maintenance
    }
    public enum IssuePriority
    {
        Urgent,
        Normal,
        Low
    }

    public enum IssueState
    {
        Unhandled,
        Handled,
        InProcess
    }
    public interface ISD
    {
       public const string XAccessToken = "X-Access-Token";
       public ClaimsPrincipal getPrincipal();
    }
    public class SD:ISD
    {
        public const string XAccessToken = "X-Access-Token";
        

       public ClaimsPrincipal getPrincipal()
        {
            var httpContextAccessor = new HttpContextAccessor();
            var token = httpContextAccessor.HttpContext?.Request.Cookies[XAccessToken];
            if (token != null)
            {
                var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                return new ClaimsPrincipal(new ClaimsIdentity(jwtSecurityToken.Claims, JwtBearerDefaults.AuthenticationScheme));
            }
            else
            {
                return new ClaimsPrincipal();
            }
        }

    }
}
