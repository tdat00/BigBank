using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeeVox.Sdk;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    [Authorize]
    public abstract class AuthenticatedController : BaseController, IController
    {
    }
}
