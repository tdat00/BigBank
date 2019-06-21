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
        public ITransactionRepository TransactionRepository {get; set;}
        public IBankAccountRepository BankAccountRepository {get; set;}
        public ICurrencyRepository CurrencyRepository {get; set;}
        public IUserRepository UserRepository {get; set;}
        public IUnitOfWork UnitOfWork {get; set;}
        public ILogger<IBankAccountService> Logger {get; set;}
        
        public BankAccountService(IUnitOfWork unitOfWork, ITransactionRepository transactionRepository, IBankAccountRepository bankAccountRepository, ICurrencyRepository currencyRepository, IUserRepository userRepository, ILogger<IBankAccountService> logger)
        {
            this.UnitOfWork = unitOfWork;
            this.TransactionRepository = transactionRepository;
            this.BankAccountRepository = bankAccountRepository;
            this.CurrencyRepository = currencyRepository;
            this.UserRepository = userRepository;
            this.Logger = logger;
        }

        public IQueryable<BankAccount> GetByUser(int userId)
        {
            return BankAccountRepository.ByUser(userId);
        }
        public IQueryable<BankAccount> GetByUser(string userEmail)
        {
            userEmail.EnsureNotNullOrWhiteSpace(nameof(userEmail));

            var user = UserRepository.ByEmail(userEmail);
            if (user == null)
            {
                throw new BusinessException($"{nameof(userEmail)} does not exist.");
            }

            return GetByUser(user.Id);
        }

        public BankAccount Get(string accountName)
        {
            accountName.EnsureNotNullOrWhiteSpace(nameof(accountName));

            var accountEntity = BankAccountRepository.ByName(accountName);
            if (accountEntity == null)
            {
                throw new BusinessException("Account name does not exist.");
            }
            return accountEntity;
        }

        public int Create(string accountNumber, string currency, int userId)
            => Create(accountNumber, currency, UserRepository.ById(userId));
        public int Create(string accountNumber, string currency, string userEmail)
            => Create(accountNumber, currency, UserRepository.ByEmail(userEmail));

        private int Create(string accountNumber, string currency, User userEntity)
        {
            accountNumber.EnsureNotNullOrWhiteSpace(nameof(accountNumber));
            currency.EnsureNotNullOrWhiteSpace(nameof(currency));

            if (userEntity == null)
            {
                throw new BusinessException("User does not exist.");
            }

            //TODO: should validate in Entity class, not this.
            accountNumber = accountNumber.Trim();
            if (accountNumber.Length < 10 && accountNumber.Length > 20)
            {
                throw new BusinessException("Account number format is not valid.");
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

        public int Deposit(User bankOfficer, string account, string currency, decimal amount, string message)
        {
            account.EnsureNotNullOrWhiteSpace(nameof(account));
            currency.EnsureNotNullOrWhiteSpace(nameof(currency));

            if (amount < 0)
            {
                throw new BusinessException("Deposit money should be greater than 0.");
            }
            
            var accountEntity = Get(account);
            var currencyEntity = CurrencyRepository.ByName(currency);
            if (currencyEntity == null)
            {
                throw new BusinessException("Currency does not exist.");
            }

            var entity = new DepositMoneyTransaction
            {
                DateTimeUtc = DateTime.Now.ToUniversalTime(),
                ToAccount = accountEntity,
                Currency = currencyEntity,
                Amount = amount,
                Message = message,
                BankOfficer = bankOfficer
            };

            TransactionRepository.Create(entity);
            UnitOfWork.SaveChanges();
            return entity.Id;
        }
    }
}
