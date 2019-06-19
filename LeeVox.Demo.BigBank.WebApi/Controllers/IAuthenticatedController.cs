using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    public interface IAuthenticatedController : IController
    {
        CurrentLoginInfo CurrentLoginInfo {get; set;}
    }
}
