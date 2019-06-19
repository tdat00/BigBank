using System;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Demo.BigBank.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    public class UserController : BaseAuthenticatedApiController, IUserController
    {
        public override CurrentLoginInfo CurrentLoginInfo {get; set;}
        public IUserService UserService {get; set;}
        public ILogger<IUserController> Logger {get; set;}

        public UserController(CurrentLoginInfo currentLoginInfo, IUserService userService, ILogger<IUserController> logger)
        {
            this.CurrentLoginInfo = currentLoginInfo;
            this.UserService = userService;
            this.Logger = logger;
        }

        public ActionResult Get([FromRoute] int id, [FromQuery] string email = null)
        {
            var user = string.IsNullOrWhiteSpace(email) ? UserService.Get(id) : UserService.Get(email);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Json(user);
            }
        }

        [HttpPut]
        public ActionResult Put([FromBody] dynamic body)
        {
            try
            {
                var user = new User
                {
                    FirstName = body.first_name ?? body.firstName,
                    LastName = body.last_name ?? body.lastName,
                    Email = body.email ?? body.Email,
                    Password = body.password ?? body.Password
                };

                var id = UserService.Create(user);
                return Ok(new {id = id});
            }
            catch (Exception ex) when (ex is ArgumentException || ex is BusinessException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while calling ~/Put: " + ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([FromBody] dynamic body)
        {
            try
            {
                string email = body.email ?? body.Email;
                string password = body.password ?? body.Password;
                var token = UserService.Login(email, password);
                if (string.IsNullOrWhiteSpace(token))
                {
                    return Unauthorized();
                }
                else
                {
                    return Json(new {token = token});
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is BusinessException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while calling ~/Login: " + ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("logout")]
        public ActionResult Logout()
        {
            try
            {
                UserService.Logout(CurrentLoginInfo.Session);
                return Ok();
            }
            catch (Exception ex) when (ex is ArgumentException || ex is BusinessException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while calling ~/Logout: " + ex.Message, ex);
                return  StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
