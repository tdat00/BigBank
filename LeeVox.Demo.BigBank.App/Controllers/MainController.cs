using System;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.App.Controllers
{
    public class MainController : IMainController
    {
        public IUserController UserController { get; set; }
        public ILogger<IMainController> Logger { get; set; }

        public MainController(IUserController userController, ILogger<IMainController> logger)
        {
            UserController = userController;
            Logger = logger;
        }

        public void Process()
        {
            Logger.LogInformation("Started app.");

            UserController.Create("Bill", "Gate", "bill.gate@microsoft.com", "P@ssw0rd");
            UserController.Create("Steve", "Jobs", "steve.jobs@apple.com", "Sup3r$ecret");

            var loginToken = UserController.Login("bill.gate@microsoft.com", "wrong-password");
            Logger.LogInformation($"Login token: {loginToken}");

            loginToken = UserController.Login("bill.gate@microsoft.com", "P@ssw0rd");
            Logger.LogInformation($"Login token: {loginToken}");

            Logger.LogInformation("Closing app...");
        }
    }
}
