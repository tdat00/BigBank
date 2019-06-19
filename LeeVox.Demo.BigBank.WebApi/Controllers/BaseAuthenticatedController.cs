using LeeVox.Demo.BigBank.Model;
using Microsoft.AspNetCore.Authorization;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    [Authorize]
    public abstract class BaseAuthenticatedController : BaseController, IAuthenticatedController
    {
        public virtual CurrentLoginInfo CurrentLoginInfo { get; set; }
    }
}
