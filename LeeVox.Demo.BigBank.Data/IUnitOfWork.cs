using System;

namespace LeeVox.Demo.BigBank.Data
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        bool Transaction(Action action);
        TResult Transaction<TResult>(Func<TResult> func);
    }
}
