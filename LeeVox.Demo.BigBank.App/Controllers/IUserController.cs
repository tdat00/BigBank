using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeeVox.Sdk;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.App.Controllers
{
    public interface IUserController
    {
        int Create(string firstName, string lastName, string email, string password);
        string Login(string email, string password);
    }
}
