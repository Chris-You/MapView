
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using CampingView.Models;
using Microsoft.Extensions.Configuration;
using System.Linq;
using StackExchange.Redis;


namespace CampingView.Services
{
    public interface IUserService
    {
        Task SignIn(HttpContext httpContext, CookieUserModel user, bool isPersistent = false);
        Task SignOut(HttpContext httpContext);

        //string GetUser(ClaimsPrincipal principal, string claim);
        //Dictionary<string, string> GetUser(ClaimsPrincipal principal);
        
    }

    public class UserService : IUserService
    {

        private readonly IConfiguration _configuration;
        private readonly RedisService _redis;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;

            _redis = new RedisService(
                           _configuration.GetSection("REDIS:SERVER").Value.ToString(),
                           _configuration.GetSection("REDIS:PORT").Value.ToString(),
                           _configuration.GetSection("REDIS:PASSWORD").Value.ToString());
        }

        /*
        public Dictionary<string, string> GetUser(ClaimsPrincipal principal)
        {
            return principal.Claims.ToDictionary(x => x.Type, x => x.Value);
        }


        public string GetUser(ClaimsPrincipal principal, string claim)
        {
            var dic = principal.Claims.ToDictionary(x => x.Type, x => x.Value);

            if (dic.ContainsKey(claim))
                return dic[claim].ToString();
            else
                return string.Empty;
        }
        */

        public async Task SignIn(HttpContext httpContext, CookieUserModel user, bool isPersistent = false)
        {
            string authenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            
              
            // Generate Claims from DbEntity
            var claims = GetUserClaims(user);

            // Add Additional Claims from the Context
            // which might be useful
            // claims.Add(httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name));

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, authenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                // AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.
                // ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.
                // IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. Required when setting the 
                // ExpireTimeSpan option of CookieAuthenticationOptions 
                // set with AddCookie. Also required when setting 
                // ExpiresUtc.
                // IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.
                // RedirectUri = "~/Account/Index"
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await httpContext.SignInAsync(authenticationScheme, claimsPrincipal, authProperties);

            this.RedisSession(user);
        }


        private void RedisSession(CookieUserModel user)
        {
            var userKey = _configuration.GetSection("REDIS:USER_KEY").Value.ToString() + ":" + user.Sns + ":" + user.Id;

            if (!_redis.redisDatabase.KeyExists(userKey))
            {
                HashEntry[] hash =
                {
                    new HashEntry("sns", user.Sns),
                    new HashEntry("id", user.Id),
                    new HashEntry("name", user.Name),
                    new HashEntry("email", user.Email)
                };

                _redis.redisDatabase.HashSet(userKey, hash);

                //var name = redis.HashGet(userKey, "name");
                //var email = redis.HashGet(userKey, "email");
            }
        }

        public async Task SignOut(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private List<Claim> GetUserClaims(CookieUserModel user)
        {
            List<Claim> claims = new List<Claim>();
            //claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
            claims.Add(new Claim("name", user.Name));
            claims.Add(new Claim("email", user.Email));
            claims.Add(new Claim("sns", user.Sns));
            claims.Add(new Claim("id", user.Id));
            return claims;
        }
    }
}
