using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    public interface IUserController : IAuthenticatedController
    {
        ActionResult Get(int? id, string email);
        ActionResult Put(dynamic body);

        ActionResult RegisterBankAccount(dynamic body);

        [AllowAnonymous]
        ActionResult Login(dynamic body);
        ActionResult Logout();
    }
}
