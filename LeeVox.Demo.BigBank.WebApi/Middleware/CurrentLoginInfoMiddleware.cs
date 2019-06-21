using Microsoft.AspNetCore.Http;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using LeeVox.Sdk;
using System.Linq;
using System.Security.Claims;

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

                var userId = authInfo?.Principal?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier || x.Type == "id")?.Value;
                var session = authInfo?.Principal?.Claims?.FirstOrDefault(x => x.Type == "session")?.Value;
                var first_name = authInfo?.Principal?.Claims?.FirstOrDefault(x => x.Type == "first_name")?.Value;
                var last_name = authInfo?.Principal?.Claims?.FirstOrDefault(x => x.Type == "last_name")?.Value;
                var email = authInfo?.Principal?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email || x.Type == "email")?.Value;
                return new CurrentLoginInfo {
                    Session = session,
                    User = new User
                    {
                        Id = userId.ParseToInt(-1),
                        FirstName = first_name,
                        LastName = last_name,
                        Email = email
                    }
                };
            });

            return services;
        }
    }
}
