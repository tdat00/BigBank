using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    public interface IUserController : IAuthenticatedController
    {
        ActionResult Get(int id, string email = null);
        ActionResult Put(dynamic body);

        ActionResult NewAccount(dynamic body);

        [AllowAnonymous]
        ActionResult Login(dynamic body);
        ActionResult Logout();
    }
}
