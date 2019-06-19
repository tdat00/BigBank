using System.Threading.Tasks;
using LeeVox.Demo.BigBank.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.WebApi.Middleware
{
    public class SessionJwtMiddleware : IMiddleware
    {
        public CurrentLoginInfo CurrentLoginInfo {get; set;}
        public IJwtSessionService JwtSessionService {get; set;}
        public ILogger<SessionJwtMiddleware> Logger {get; set;}
        public SessionJwtMiddleware(CurrentLoginInfo currentLoginInfo, IJwtSessionService jwtSessionService, ILogger<SessionJwtMiddleware> logger)
        {
            this.CurrentLoginInfo = currentLoginInfo;
            this.JwtSessionService = jwtSessionService;
            this.Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var session = CurrentLoginInfo.Session ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(session) && !JwtSessionService.ContainsKey(session))
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else
            {
                await next(context);
            }
        }
    }
}
