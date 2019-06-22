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

        [HttpPut]
        [Authorize(Roles = "Admin,BankOfficer")]
        public ActionResult Put([FromBody] dynamic body)
        {
            try
            {
                string email = body.email ?? body.Email;
                string password = body.password ?? body.Password;
                string role = body.role ?? body.Role;
                string firstName = body.first_name ?? body.firstName ?? body.FirstName;
                string lastName = body.last_name ?? body.lastName ?? body.LastName;

                string bankAccount = body.account ?? body.Account ?? body.account_name ?? body.accountName ?? body.AccountName;
                string bankAccountCurrency = body.account_currency ?? body.accountCurrency ?? body.AccountCurrency;

                var (userId, accountId) = UserService.Create(email, password, role, firstName, lastName, bankAccount, bankAccountCurrency);
                return Ok(new {user = userId, account = accountId});
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
        [Authorize(Roles = "BankOfficer")]
        public ActionResult RegisterBankAccount([FromBody] dynamic body)
        {
            try
            {
                string account = body.account ?? body.Account ?? body.account_name ?? body.accountName ?? body.AccountName;
                string currency = body.currency;
                int? userId = ((string)(body.user_id ?? body.userId ?? body.user)).ParseToInt();
                string email = body.user_email ?? body.userEmail ?? body.email ?? body.user;
                
                var id = userId.HasValue
                    ? BankAccountService.Create(account, currency, userId.Value)
                    : BankAccountService.Create(account, currency, email);
                return Ok(new {account = id});
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
