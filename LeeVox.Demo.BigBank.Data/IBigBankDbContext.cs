using LeeVox.Demo.BigBank.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LeeVox.Demo.BigBank.Data
{
    public interface IBigBankDbContext : IUnitOfWork
    {
        DbSet<User> Users {get; }
        DbSet<Transaction> Transactions {get; }
        DbSet<BankAccount> BankAccounts {get; }
        DbSet<Currency> Currencies {get; }
        DbSet<ExchangeRateHistory> ExchangeRateHistories {get; }

        DbSet<TEntity> GetDbSet<TEntity>() where TEntity: class, IEntity;

        EntityEntry<TEntity> AttachEntity<TEntity>(TEntity entity) where TEntity: class, IEntity;
    }
}
