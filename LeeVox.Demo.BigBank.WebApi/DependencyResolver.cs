using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Service;
using LeeVox.Demo.BigBank.WebApi.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace LeeVox.Demo.BigBank.WebApi
{
    public class DependencyResolver
    {
        public void AddDependencies(IServiceCollection services)
        {
            // LeeVox.Demo.Bigbank.WebApi
            services.AddScoped<IUserController, UserController>();

            // LeeVox.Demo.Bigbank.Service
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILoginInfoService, LoginInfoService>();

            // LeeVox.Demo.Bigbank.Data
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddDbContext<InMemoryBigBankDbContext>();
            services.AddScoped<IBigBankDbContext>(x => x.GetRequiredService<InMemoryBigBankDbContext>());
            services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<InMemoryBigBankDbContext>());
        }
    }
}
