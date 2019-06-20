using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    public interface IExchangeRateController : IAuthenticatedController
    {
        ActionResult Insert(dynamic body);
    }
}
