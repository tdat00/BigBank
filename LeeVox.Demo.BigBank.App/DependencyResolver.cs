using LeeVox.Demo.BigBank.App.Controllers;
using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace LeeVox.Demo.BigBank.App
{
    public class DependencyResolver
    {
        public void AddDependencies(IServiceCollection services)
        {
            // LeeVox.Demo.BigBank.App
            services.AddScoped<IMainController, MainController>();
            services.AddScoped<IUserController, UserController>();

            // LeeVox.Demo.Bigbank.Service
            services.AddScoped<IUserService, UserService>();

            // LeeVox.Demo.Bigbank.Data
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddDbContext<InMemoryBigBankDbContext>();
            services.AddScoped<IBigBankDbContext>(x => x.GetRequiredService<InMemoryBigBankDbContext>());
            services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<InMemoryBigBankDbContext>());
        }
    }
}
