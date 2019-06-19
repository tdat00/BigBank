using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    public interface IUserController : IApiController
    {
        ActionResult Get(int id, string email = null);
        ActionResult Put(dynamic body);

        [AllowAnonymous]
        ActionResult Login(dynamic body);
        ActionResult Logout(dynamic body);
    }
}
