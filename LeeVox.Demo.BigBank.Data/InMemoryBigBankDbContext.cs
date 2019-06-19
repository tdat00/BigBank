using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

using LeeVox.Demo.BigBank.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using LeeVox.Demo.BigBank.Core;

namespace LeeVox.Demo.BigBank.Data
{
    public class InMemoryBigBankDbContext : DbContext, IBigBankDbContext, IUnitOfWork
    {
        public DbSet<User> Users {get; set;}

        public InMemoryBigBankDbContext()
        {
            Database.EnsureCreated();
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
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash
            });
#endif
        }

        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity: class, IEntity
            => this.Set<TEntity>();

        public EntityEntry<TEntity> AttachEntity<TEntity>(TEntity entity) where TEntity: class, IEntity
            => this.Attach(entity);
    }
}
