using LeeVox.Demo.BigBank.Model;
using Microsoft.EntityFrameworkCore;

namespace LeeVox.Demo.BigBank.Data
{
    public interface IBigBankDbContext : IUnitOfWork
    {
        DbSet<User> Users {get; }
        DbSet<Transaction> Transactions {get; }
        DbSet<DepositMoneyTransaction> DepositMoneyTransactions {get; }
        DbSet<WithdrawMoneyTransaction> WithdrawMoneyTransactions {get; }
        DbSet<TransferMoneyTransaction> TransferMoneyTransactions {get; }
        DbSet<BankAccount> BankAccounts {get; }
        DbSet<Currency> Currencies {get; }
        DbSet<ExchangeRateHistory> ExchangeRateHistories {get; }

        DbSet<TEntity> GetDbSet<TEntity>() where TEntity: class, IEntity;
    }
}
