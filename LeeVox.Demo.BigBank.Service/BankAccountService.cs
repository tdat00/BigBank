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
            entity.EnsureNotNull("Account");

            return entity;
        }

        public BankAccount Get(string accountName)
        {
            accountName.EnsureNotNullOrWhiteSpace(nameof(accountName));
            
            var entity = BankAccountRepository.ByAccountName(accountName);
            entity.EnsureNotNull("Account");
            
            return entity;
        }

        public bool Exists(string accountName)
            => BankAccountRepository.ByAccountName(accountName) != null;
        
        public bool IsOwnedByUser(string accountName, int userId)
            => IsOwnedByUser(BankAccountRepository.ByAccountName(accountName), UserRepository.ById(userId));

        public bool IsOwnedByUser(string accountName, string email)
            => IsOwnedByUser(BankAccountRepository.ByAccountName(accountName), UserRepository.ByEmail(email));
        public bool IsOwnedByUser(string accountName, User user)
            => IsOwnedByUser(BankAccountRepository.ByAccountName(accountName), user);
        public bool IsOwnedByUser(BankAccount account, User user)
            => account != null && user != null && account.UserId == user.Id;

        public IQueryable<BankAccount> GetByUser(int userId)
            => BankAccountRepository.All.Where(x => x.UserId == userId);

        public IQueryable<BankAccount> GetByUser(string userEmail)
            => BankAccountRepository.All.Where(x => userEmail.IsOrdinalEqual(x.User.Email, true));

        public int Create(string accountName, string currency, int userId)
            => Create(accountName, currency, UserRepository.ById(userId));
        public int Create(string accountName, string currency, string userEmail)
            => Create(accountName, currency, UserRepository.ByEmail(userEmail));

        private int Create(string accountName, string currency, User userEntity)
        {
            accountName.EnsureNotNullOrWhiteSpace(nameof(accountName));
            currency.EnsureNotNullOrWhiteSpace(nameof(currency));
            userEntity.EnsureNotNull("User");

            //TODO: should validate in Entity class, not this.
            accountName = accountName.Trim();
            if (accountName.Length < 10 && accountName.Length > 20)
            {
                throw new BusinessException("Account name is not valid.");
            }

            if (BankAccountRepository.ByAccountName(accountName) != null)
            {
                throw new BusinessException("Bank account name is taken.");
            }

            var currencyEntity = CurrencyService.Get(currency);
            currencyEntity.EnsureNotNull("Currency");

            var entity = new BankAccount
            {
                AccountName = accountName,
                Currency = currencyEntity,
                User = userEntity
            };

            BankAccountRepository.Create(entity);
            UnitOfWork.SaveChanges();
            return entity.Id;
        }
    }
}
