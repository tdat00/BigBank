using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeeVox.Sdk;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Demo.BigBank.Service;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.App.Controllers
{
    public class UserController : IUserController
    {
        public IUserService UserService { get; set; }
        public ILogger<UserController> Logger { get; set; }

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            UserService = userService;
            Logger = logger;
        }

        public int Create(string firstName, string lastName, string email, string password)
        {
            var id = UserService.Create(new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password
            });
            Logger.LogInformation("Created user with Id: " + id);
            return id;
        }

        public string Login(string email, string password)
        {
            try
            {
                return UserService.Login(email, password);
            }
            catch (Exception ex)
            {
                Logger.LogError("Login failed: " + ex.Message);
                return string.Empty;
            }
        }

        public void Delete(string email)
        {
            try
            {
                UserService.Delete(email);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while deleting: " + ex.Message);
            }
        }
    }
}
