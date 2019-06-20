using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Service;
using LeeVox.Demo.BigBank.WebApi.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace LeeVox.Demo.BigBank.WebApi
{
    public class DependencyResolver
    {
        public void AddDependencies(IServiceCollection services)
        {
            // LeeVox.Demo.Bigbank.WebApi.Controller
            services.AddScoped<IUserController, UserController>();
            services.AddScoped<IExchangeRateController, ExchangeRateController>();
            services.AddScoped<IBankAccountController, BankAccountController>();

            // LeeVox.Demo.Bigbank.Service
            services.AddSingleton<IJwtSessionService, JwtSessionService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IExchangeRateHistoryService, ExchangeRateHistoryService>();
            services.AddScoped<ITransactionService, TransactionService>();

            // LeeVox.Demo.Bigbank.Data
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IExchangeRateHistoryRepository, ExchangeRateHistoryRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            
            services.AddDbContext<InMemoryBigBankDbContext>();
            services.AddScoped<IBigBankDbContext>(x => x.GetRequiredService<InMemoryBigBankDbContext>());
            services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<InMemoryBigBankDbContext>());
        }
    }
}
