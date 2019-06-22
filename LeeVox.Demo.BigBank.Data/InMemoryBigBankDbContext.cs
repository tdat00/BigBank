using Microsoft.EntityFrameworkCore;

using LeeVox.Demo.BigBank.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using LeeVox.Demo.BigBank.Core;
using System;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public class InMemoryBigBankDbContext : DbContext, IBigBankDbContext, IUnitOfWork
    {
        public DbSet<User> Users {get; set;}
        public DbSet<Transaction> Transactions {get; }
        public DbSet<BankAccount> BankAccounts {get; }
        public DbSet<Currency> Currencies {get; }
        public DbSet<ExchangeRateHistory> ExchangeRateHistories {get; }

        public ILogger<IBigBankDbContext> Logger {get; set;}

        public InMemoryBigBankDbContext(ILogger<IBigBankDbContext> logger)
        {
            Database.EnsureCreated();

            this.Logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseInMemoryDatabase("in_memory_big_bank_db");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
#if DEBUG
            // create super admin account
            var random = new CryptoRandom();
            var hasher = new CryptoHash();
            var passwordSalt = random.RandomBytes(16).GetHexaString();
            var passwordHash = hasher.Sha256(passwordSalt + "T0p$ecret").GetHexaString();
            builder.Entity<User>().HasData(new User
            {
                Id = 1000,
                FirstName = "Super",
                LastName = "Admin",
                Email = "admin@big.bank",
                Role = UserRole.Admin | UserRole.BankOfficer,
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash
            });

            // create currencies
            builder.Entity<Currency>().HasData(new Currency[]
            {
                new Currency { Id = 1000, Name = "VND" },
                new Currency { Id = 1001, Name = "USD" },
                new Currency { Id = 1002, Name = "EUR" }
            });
#endif
        }

        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity: class, IEntity
            => this.Set<TEntity>();

        public EntityEntry<TEntity> AttachEntity<TEntity>(TEntity entity) where TEntity: class, IEntity
            => this.Attach(entity);

        public bool Transaction(Action action)
        {
            bool result = true;
            using (var transaction = this.Database.BeginTransaction())
            {
                try
                {
                    Logger.LogInformation($"Begin transaction {transaction.TransactionId}.");
                    
                    action.Invoke();
                    transaction.Commit();
                    
                    Logger.LogInformation($"Successfully committed transaction {transaction.TransactionId}.");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error while running transaction {transaction.TransactionId}: {ex.Message}", ex);
                    transaction.Rollback();

                    result = false;
                }
            }
            return result;
        }

        public TResult Transaction<TResult>(Func<TResult> func)
        {
            TResult result;
            using (var transaction = this.Database.BeginTransaction())
            {
                try
                {
                    Logger.LogInformation($"Begin transaction {transaction.TransactionId}.");
                    
                    result = func.Invoke();
                    transaction.Commit();

                    Logger.LogInformation($"Successfully committed transaction {transaction.TransactionId}.");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error while running transaction {transaction.TransactionId}: {ex.Message}", ex);
                    transaction.Rollback();

                    result = default(TResult);
                }
            }
            return result;
        }
    }
}
