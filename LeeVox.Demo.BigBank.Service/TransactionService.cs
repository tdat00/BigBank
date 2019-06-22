using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Sdk;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Service
{
    public class TransactionService : ITransactionService
    {
        public IBankAccountService BankAccountService {get; set;}
        public ICurrencyService CurrencyService {get; set;}
        public IExchangeRateHistoryService ExchangeRateHistoryService {get; set;}
        public IRepository<Transaction> TransactionRepository {get; set;}
        public IBankAccountRepository BankAccountRepository {get; set;}
        public IUnitOfWork UnitOfWork {get; set;}
        public ILogger<IBankAccountService> Logger {get; set;}
        
        public TransactionService(
            IBankAccountService bankAccountService,
            ICurrencyService currencyService,
            IExchangeRateHistoryService exchangeRateHistoryService,

            IRepository<Transaction> transactionRepository,
            IBankAccountRepository bankAccountRepository,

            IUnitOfWork unitOfWork,
            ILogger<IBankAccountService> logger
        )
        {
            this.BankAccountService = bankAccountService;
            this.CurrencyService = currencyService;
            this.ExchangeRateHistoryService = exchangeRateHistoryService;

            this.TransactionRepository = transactionRepository;
            this.BankAccountRepository = bankAccountRepository;

            this.UnitOfWork = unitOfWork;
            this.Logger = logger;
        }
        
        public (int transactionId, decimal exchangeRate, decimal delta, decimal newBalance) Deposit(User bankOfficer, string account, string currency, decimal money, string message)
        {
            if (money < 0)
            {
                throw new BusinessException("Money should be greater than 0.");
            }

            var nowUtc = DateTime.Now.ToUniversalTime();
            var accountEntity = BankAccountService.Get(account);
            var currencyEntity = CurrencyService.Get(currency);

            var exchangeRate = ExchangeRateHistoryService.GetExchangeRate(currencyEntity, accountEntity.Currency, nowUtc);
            var delta = money * exchangeRate;

            if (delta <= 0)
            {
                throw new BusinessException("Deposit delta is not valid.");
            }

            accountEntity.Balance += delta;

            UnitOfWork.AttachEntity(bankOfficer).State = EntityState.Unchanged;

            var entity = new DepositMoneyTransaction
            {
                DateTimeUtc = nowUtc,
                Account = accountEntity,
                Currency = currencyEntity,
                Money = money,
                Message = message,
                BankOfficer = bankOfficer
            };

            TransactionRepository.Create(entity);
            BankAccountRepository.Update(accountEntity);
            
            UnitOfWork.SaveChanges();
            return (entity.Id, exchangeRate, delta, accountEntity.Balance);
        }

        public (int transactionId, decimal exchangeRate, decimal delta, decimal newBalance) Withdraw(string accountName, string currency, decimal money, string message)
        {
            if (money < 0)
            {
                throw new BusinessException("Money should be greater than 0.");
            }

            var nowUtc = DateTime.Now.ToUniversalTime();
            var accountEntity = BankAccountService.Get(accountName);
            var currencyEntity = CurrencyService.Get(currency);

            var exchangeRate = ExchangeRateHistoryService.GetExchangeRate(currencyEntity, accountEntity.Currency, nowUtc);
            var delta = money * exchangeRate;

            if (delta > accountEntity.Balance)
            {
                throw new BusinessException("Account balance is not sufficient");
            }
            else if (delta <= 0)
            {
                throw new BusinessException("Withdraw delta is not valid.");
            }

            accountEntity.Balance -= delta;

            var entity = new WithdrawMoneyTransaction
            {
                DateTimeUtc = nowUtc,
                Account = accountEntity,
                Currency = currencyEntity,
                Money = money,
                Message = message
            };

            TransactionRepository.Create(entity);
            BankAccountRepository.Update(accountEntity);
            
            UnitOfWork.SaveChanges();
            return (entity.Id, exchangeRate, delta, accountEntity.Balance);
        }

        public (
            int transactionId,
            decimal exchangeRateFromSourceAccount, decimal deltaFromSourceAccount, decimal newSourceAccountBalance,
            decimal exchangeRateToTargetAccount, decimal deltaToTargetAccount, decimal newTargetAccountBalance
        ) Transfer(string fromAccount, string toAccount, string currency, decimal money, string message)
        {
            if (money < 0)
            {
                throw new BusinessException("Money should be greater than 0.");
            }

            var nowUtc = DateTime.Now.ToUniversalTime();
            var sourceAccountEntity = BankAccountService.Get(fromAccount);
            var targetAccountEntity = BankAccountService.Get(toAccount);
            var currencyEntity = CurrencyService.Get(currency);

            var exchangeRateFromSourceAccount = ExchangeRateHistoryService.GetExchangeRate(sourceAccountEntity.Currency, currencyEntity, nowUtc);
            var exchangeRateToTargetAccount = ExchangeRateHistoryService.GetExchangeRate(currencyEntity, targetAccountEntity.Currency, nowUtc);
            var deltaFromSourceAccount = money * exchangeRateFromSourceAccount;
            var deltaToTargetAccount = money * exchangeRateToTargetAccount;

            if (deltaFromSourceAccount > sourceAccountEntity.Balance)
            {
                throw new BusinessException("Source account balance is not sufficient");
            }
            else if (deltaFromSourceAccount <= 0 || deltaToTargetAccount <= 0)
            {
                throw new BusinessException("Transfer delta is not valid.");
            }

            sourceAccountEntity.Balance -= deltaFromSourceAccount;
            targetAccountEntity.Balance += deltaToTargetAccount;

            var entity = new TransferMoneyTransaction
            {
                DateTimeUtc = nowUtc,
                SourceAccount = sourceAccountEntity,
                Account = targetAccountEntity,
                Currency = currencyEntity,
                Money = money,
                Message = message
            };

            TransactionRepository.Create(entity);
            BankAccountRepository.Update(sourceAccountEntity);
            BankAccountRepository.Update(targetAccountEntity);
            
            UnitOfWork.SaveChanges();
            return (
                entity.Id,
                exchangeRateFromSourceAccount, deltaFromSourceAccount, sourceAccountEntity.Balance,
                exchangeRateToTargetAccount, deltaToTargetAccount, targetAccountEntity.Balance
            );
        }

        public (DateTime from, DateTime to, IEnumerable<Transaction> transactions) QueryTransactions(string account, string fromDate, string toDate)
        {
            var from = fromDate.ParseToDateTime(DateTimeStyles.AssumeUniversal, null, DateTime.Now.Date.AddMonths(-3)).ToUniversalTime();
            var to = toDate.ParseToDateTime(DateTimeStyles.AssumeUniversal, null, DateTime.Now.Date.AddDays(1).AddMilliseconds(-1)).ToUniversalTime();

            var accountEntity = BankAccountService.Get(account);

            var depositTransactions = TransactionRepository.All.OfType<DepositMoneyTransaction>().Where(x => x.DateTimeUtc >= from && x.DateTimeUtc <= to && x.AccountId == accountEntity.Id).IncludeProperty(x => x.Currency);
            var withdrawTransactions = TransactionRepository.All.OfType<WithdrawMoneyTransaction>().Where(x => x.DateTimeUtc >= from && x.DateTimeUtc <= to && x.AccountId == accountEntity.Id).IncludeProperty(x => x.Currency);
            var transferTransactions = TransactionRepository.All.OfType<TransferMoneyTransaction>().Where(x => x.DateTimeUtc >= from && x.DateTimeUtc <= to && (x.AccountId == accountEntity.Id || x.SourceAccountId == accountEntity.Id)).IncludeProperty(x => x.Currency);

            //TODO: beware out of memory exception.
            var result = new List<Transaction>();
            result.AddRange(depositTransactions);
            result.AddRange(withdrawTransactions);
            result.AddRange(transferTransactions);

            return (from, to, result.OrderByDescending(x => x.DateTimeUtc));
        }
    }
}
