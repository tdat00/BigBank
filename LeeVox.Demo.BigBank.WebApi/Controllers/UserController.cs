using System;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Demo.BigBank.Service;
using LeeVox.Sdk;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    public class UserController : BaseAuthenticatedApiController, IUserController
    {
        public IUserService UserService {get; set;}
        public IBankAccountService BankAccountService {get; set;}
        public ILogger<IUserController> Logger {get; set;}

        public UserController(CurrentLoginInfo currentLoginInfo, IUserService userService, IBankAccountService bankAccountService, ILogger<IUserController> logger)
        {
            this.CurrentLoginInfo = currentLoginInfo;
            this.UserService = userService;
            this.BankAccountService = bankAccountService;
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
                string email = body.email ?? body.Email;
                string password = body.password ?? body.Password;
                string firstName = body.first_name ?? body.firstName;
                string lastName = body.last_name ?? body.lastName;
                string bankAccount = body.account_number ?? body.accountNumber;
                string bankAccountCurrency = body.account_currency ?? body.accountCurrency;

                var id = UserService.Create(email, password, firstName, lastName, bankAccount, bankAccountCurrency);
                return Ok(new {id = id});
            }
            catch (Exception ex) when (ex is ArgumentException || ex is BusinessException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while inserting new user: " + ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPut("register-bank-account")]
        public ActionResult RegisterBankAccount([FromBody] dynamic body)
        {
            try
            {
                string account = body.account_number ?? body.accountNumber ?? body.account;
                string currency = body.currency;
                int? userId = ((string)(body.user_id ?? body.userId ?? body.user)).ParseToInt();
                string email = body.user_email ?? body.userEmail ?? body.email ?? body.user;
                
                var id = userId.HasValue
                    ? BankAccountService.Create(account, currency, userId.Value)
                    : BankAccountService.Create(account, currency, email);
                return Ok(new {id = id});
            }
            catch (Exception ex) when (ex is ArgumentException || ex is BusinessException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while registering new bank account: " + ex.Message, ex);
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
                Logger.LogError("Error while logging in: " + ex.Message, ex);
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
                Logger.LogError("Error while logging out: " + ex.Message, ex);
                return  StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
