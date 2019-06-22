using System;
using LeeVox.Demo.BigBank.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LeeVox.Demo.BigBank.Data
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        EntityEntry<TEntity> AttachEntity<TEntity>(TEntity entity) where TEntity: class, IEntity;
        bool Transaction(Action action);
        TResult Transaction<TResult>(Func<TResult> func);
    }
}
