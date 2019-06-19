using LeeVox.Demo.BigBank.Model;
using Microsoft.AspNetCore.Authorization;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    [Authorize]
    public abstract class BaseAuthenticatedApiController : BaseApiController, IAuthenticatedController
    {
        public virtual CurrentLoginInfo CurrentLoginInfo { get; set; }
    }
}
