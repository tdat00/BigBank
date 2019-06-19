using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    [Authorize]
    public abstract class AuthenticatedApiController : BaseApiController, IApiController
    {
    }
}
