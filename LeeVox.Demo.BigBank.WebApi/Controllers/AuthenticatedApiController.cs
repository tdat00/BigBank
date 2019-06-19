using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeeVox.Sdk;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    [Authorize]
    public abstract class AuthenticatedApiController : BaseApiController, IApiController
    {
    }
}
