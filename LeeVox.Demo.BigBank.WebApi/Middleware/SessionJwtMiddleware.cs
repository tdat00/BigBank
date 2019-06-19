using System.Threading.Tasks;
using LeeVox.Demo.BigBank.Service;
using Microsoft.AspNetCore.Http;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.WebApi.Middleware
{
    public class SessionJwtMiddleware
    {
        public RequestDelegate Next {get; set;}
        public SessionJwtMiddleware(RequestDelegate next)
        {
            Next = next;
        }
        public async Task InvokeAsync(HttpContext context, CurrentLoginInfo currentLoginInfo, IJwtSessionService jwtSessionService)
        {
            var session = currentLoginInfo.Session ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(session) && !jwtSessionService.ContainsKey(session))
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else
            {
                await Next(context);
            }
        }
    }
}
