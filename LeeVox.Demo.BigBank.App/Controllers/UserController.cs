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
        public ILogger<IUserController> Logger { get; set; }

        public UserController(IUserService userService, ILogger<IUserController> logger)
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
                Password = password,
                PasswordHash = password + "-hashed"
            });
            Logger.LogInformation("New ID: " + id);
            return id;
        }

        public string Login(string email, string password)
        {
            var token = UserService.Login(email, password);
            if (string.IsNullOrEmpty(token))
            {
                Logger.LogError("Login failed.");
            }
            return token;
        }
    }
}
