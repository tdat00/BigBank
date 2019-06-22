using System.Collections.Generic;
using System.Linq;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Sdk;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Service
{
    public class BankAccountService : IBankAccountService
    {
        public ICurrencyService CurrencyService {get; set;}
        public IBankAccountRepository BankAccountRepository {get; set;}
        public IUserRepository UserRepository {get; set;}
        public IUnitOfWork UnitOfWork {get; set;}
        public ILogger<IBankAccountService> Logger {get; set;}
        
        public BankAccountService(
            ICurrencyService currencyService,

            IBankAccountRepository bankAccountRepository,
            IUserRepository userRepository,

            IUnitOfWork unitOfWork,
            ILogger<IBankAccountService> logger
        )
        {
            this.CurrencyService = currencyService;

            this.BankAccountRepository = bankAccountRepository;
            this.UserRepository = userRepository;

            this.UnitOfWork = unitOfWork;
            this.Logger = logger;
        }

        public BankAccount Get(int id)
        {
            var entity = BankAccountRepository.ById(id);
            entity.EnsureNotNull("Bank account");

            return entity;
        }

        public BankAccount Get(string accountNumber)
        {
            accountNumber.EnsureNotNullOrWhiteSpace(nameof(accountNumber));
            
            var entity = BankAccountRepository.All.FirstOrDefault(x => accountNumber.IsOrdinalEqual(x.AccountNumber, true));
            entity.EnsureNotNull("Bank account");
            
            return entity;
        }

        public IQueryable<BankAccount> GetByUser(int userId)
            => BankAccountRepository.All.Where(x => x.UserId == userId);

        public IQueryable<BankAccount> GetByUser(string userEmail)
            => BankAccountRepository.All.IncludeProperty(x => x.User).Where(x => userEmail.IsOrdinalEqual(x.User.Email, true));

        public int Create(string accountNumber, string currency, int userId)
            => Create(accountNumber, currency, UserRepository.ById(userId));
        public int Create(string accountNumber, string currency, string userEmail)
            => Create(accountNumber, currency, UserRepository.ByEmail(userEmail));

        private int Create(string accountNumber, string currency, User userEntity)
        {
            accountNumber.EnsureNotNullOrWhiteSpace(nameof(accountNumber));
            currency.EnsureNotNullOrWhiteSpace(nameof(currency));
            userEntity.EnsureNotNull("User");

            //TODO: should validate in Entity class, not this.
            accountNumber = accountNumber.Trim();
            if (accountNumber.Length < 10 && accountNumber.Length > 20)
            {
                throw new BusinessException("Account number format is not valid.");
            }

            var currencyEntity = CurrencyService.Get(currency);
            currencyEntity.EnsureNotNull("Currency");

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
