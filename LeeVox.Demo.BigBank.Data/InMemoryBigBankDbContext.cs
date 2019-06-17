using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

using LeeVox.Demo.BigBank.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LeeVox.Demo.BigBank.Data
{
    public class InMemoryBigBankDbContext : DbContext, IBigBankDbContext, IUnitOfWork
    {
        public DbSet<User> Users {get; set;}

        public InMemoryBigBankDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("in_memory_big_bank_db");
        }

        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity: class, IEntity
            => this.Set<TEntity>();

        public EntityEntry<TEntity> AttachEntity<TEntity>(TEntity entity) where TEntity: class, IEntity
            => this.Attach(entity);
    }
}
