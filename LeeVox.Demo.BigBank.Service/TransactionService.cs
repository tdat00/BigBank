using System;
using System.Collections.Generic;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Service
{
    public class TransactionService : ITransactionService
    {
        public IBankAccountService BankAccountService {get; set;}
        public ICurrencyService CurrencyService {get; set;}
        public ITransactionRepository TransactionRepository {get; set;}
        public IUnitOfWork UnitOfWork {get; set;}
        public ILogger<IBankAccountService> Logger {get; set;}
        
        public TransactionService(
            IBankAccountService bankAccountService,
            ICurrencyService currencyService,

            ITransactionRepository transactionRepository,
            IBankAccountRepository bankAccountRepository,
            ICurrencyRepository currencyRepository,

            IUnitOfWork unitOfWork,
            ILogger<IBankAccountService> logger
        )
        {
            this.BankAccountService = bankAccountService;
            this.CurrencyService = currencyService;

            this.TransactionRepository = transactionRepository;

            this.UnitOfWork = unitOfWork;
            this.Logger = logger;
        }
        
        public int Deposit(User bankOfficer, string account, string currency, decimal money, string message)
        {
            var accountEntity = BankAccountService.Get(account);
            var currencyEntity = CurrencyService.Get(currency);

            if (money < 0)
            {
                throw new BusinessException("Deposit money should be greater than 0.");
            }

            var entity = new DepositMoneyTransaction
            {
                DateTimeUtc = DateTime.Now.ToUniversalTime(),
                Account = accountEntity,
                Currency = currencyEntity,
                Money = money,
                Message = message,
                BankOfficer = bankOfficer
            };

            TransactionRepository.Create(entity);
            UnitOfWork.SaveChanges();
            return entity.Id;
        }

        public int Withdraw(string account, decimal money, string message)
        {
            throw new System.NotImplementedException();
        }
    }
}
