using System;
using System.Collections.Generic;
using System.Linq;
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

        public BankAccount Get(string accountNumber)
        {
            var accountEntity = BankAccountRepository.ByAccountNumber(accountNumber);
            if (accountEntity == null)
            {
                throw new BusinessException("Account number does not exist.");
            }
            return accountEntity;
        }

        public int Create(string accountNumber, string currency, int userId)
            => Create(accountNumber, currency, UserRepository.ById(userId));
        public int Create(string accountNumber, string currency, string userEmail)
            => Create(accountNumber, currency, UserRepository.ByEmail(userEmail));

        private int Create(string accountNumber, string currency, User userEntity)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                throw new ArgumentException("Account number is required.");
            }
            //TODO: should validate in Entity class, not this.
            accountNumber = accountNumber.Trim();
            if (accountNumber.Length < 10 && accountNumber.Length > 20)
            {
                throw new BusinessException("Account number format is not valid.");
            }
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("Currency is required.");
            }

            if (userEntity == null)
            {
                throw new BusinessException("User does not exist.");
            }

            var currencyEntity = CurrencyRepository.ByName(currency);
            if (currencyEntity == null)
            {
                throw new BusinessException("Currency does not exist.");
            }

            var entity = new BankAccount
            {
                AccountNumber = accountNumber,
                Currency = currencyEntity,
                User = userEntity
            };

            BankAccountRepository.Create(entity);
            UnitOfWork.SaveChanges();
            return entity.Id;
        }
    }
}
