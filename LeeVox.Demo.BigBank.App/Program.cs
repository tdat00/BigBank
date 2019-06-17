using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LeeVox.Sdk;
using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Demo.BigBank.Service;
using LeeVox.Demo.BigBank.App.Controllers;

namespace LeeVox.Demo.BigBank.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging(configure => {
                configure.AddConsole().SetMinimumLevel(LogLevel.Information);
            });

            // Dependency injection
            new DependencyResolver().AddDependencies(services);
            var provider = services.BuildServiceProvider();

            provider.GetService<IMainController>().Process();
            provider.Dispose();
        }
    }
}
