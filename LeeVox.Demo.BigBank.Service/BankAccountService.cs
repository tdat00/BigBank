using System;
using System.Collections.Generic;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Service
{
    public class BankAccountService : IBankAccountService
    {
        public IBankAccountRepository BankAccountRepository {get; set;}
        public ICurrencyRepository CurrencyRepository {get; set;}
        public IUserRepository UserRepository {get; set;}
        public IUnitOfWork UnitOfWork {get; set;}
        public ILogger<IBankAccountService> Logger {get; set;}
        
        public BankAccountService(IUnitOfWork unitOfWork, IBankAccountRepository bankAccountRepository, ICurrencyRepository currencyRepository, IUserRepository userRepository, ILogger<IBankAccountService> logger)
        {
            this.UnitOfWork = unitOfWork;
            this.BankAccountRepository = bankAccountRepository;
            this.CurrencyRepository = currencyRepository;
            this.UserRepository = userRepository;
            this.Logger = logger;
        }

        public int Create(BankAccount account)
        {
            if (account.User == null || string.IsNullOrWhiteSpace(account.User.Email))
            {
                throw new ArgumentException("User email is required.");
            }
            if (account.Currency == null || string.IsNullOrWhiteSpace(account.Currency.Name))
            {
                throw new ArgumentException("Currency is required.");
            }

            var currency = CurrencyRepository.ByName(account.Currency.Name);
            var user = UserRepository.ByEmail(account.User.Email);

            if (currency == null)
            {
                throw new BusinessException("Currency does not exist.");
            }
            if (user == null)
            {
                throw new BusinessException("User does not exist.");
            }

            var entity = new BankAccount
            {
                AccountNumber = account.AccountNumber,
                Currency = currency,
                User = user
            };

            BankAccountRepository.Create(entity);
            UnitOfWork.SaveChanges();
            return entity.Id;
        }
    }
}
