using Microsoft.AspNetCore.Http;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using LeeVox.Sdk;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using System;

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

                var session = claims.FirstOrDefault(x => "session".IsOrdinalEqual(x.Type, true))?.Value;
                var userId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier || "id".IsOrdinalEqual(x.Type, true))?.Value;
                var first_name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name || "first_name".IsOrdinalEqual(x.Type, true))?.Value;
                var last_name = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName || "last_name".IsOrdinalEqual(x.Type, true))?.Value;
                var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email || "email".IsOrdinalEqual(x.Type, true))?.Value;
                
                var roleClaims = claims.Where(x => x.Type == ClaimTypes.Role || "role".IsOrdinalEqual(x.Type, true)) ?? new List<Claim>();
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
