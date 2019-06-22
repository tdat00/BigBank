using Microsoft.AspNetCore.Http;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using LeeVox.Sdk;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace LeeVox.Demo.BigBank.WebApi.Middleware
{
    public static class CurrentLoginInfoMiddlewareExtension
    {
        public static IServiceCollection AddCurrentLoginInfo(this IServiceCollection services)
        {
            services.AddScoped<CurrentLoginInfo>(provider =>
            {
                var httpContext = provider.GetService<IHttpContextAccessor>();
                var authInfo = httpContext.HttpContext.AuthenticateAsync().WaitAndReturn();
                var claims = authInfo?.Principal?.Claims ?? new List<Claim>();

                // foreach (var claim in claims)
                // {
                //     Console.WriteLine($"++++++++++ {claim.Type} = {claim.Value}");
                // }

                var session = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
                var userId = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub || x.Type == ClaimTypes.NameIdentifier)?.Value;
                var first_name = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.GivenName || x.Type == ClaimTypes.GivenName)?.Value;
                var last_name = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.FamilyName || x.Type == ClaimTypes.Surname)?.Value;
                var email = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email || x.Type == ClaimTypes.Email)?.Value;
                
                var roleClaims = claims.Where(x => x.Type == ClaimTypes.Role) ?? new List<Claim>();
                var roles = string.Join(",", roleClaims.Select(x => x.Value));
                if (!Enum.TryParse(roles, out UserRole userRole))
                    userRole = UserRole.Customer;
                    
                return new CurrentLoginInfo {
                    Session = session,
                    User = new User
                    {
                        Id = userId.ParseToInt(-1),
                        FirstName = first_name,
                        LastName = last_name,
                        Email = email,
                        Role = userRole
                    }
                };
            });

            return services;
        }
    }
}
