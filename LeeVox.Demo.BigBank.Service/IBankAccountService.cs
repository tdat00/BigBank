using System.Collections.Generic;
using System.Linq;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public interface IBankAccountService : IService
    {
        BankAccount Get(int id);
        BankAccount Get(string accountName);
        bool Exists(string accountName);
        bool IsOwnedByUser(string accountName, int userId);
        bool IsOwnedByUser(string accountName, string userEmail);
        bool IsOwnedByUser(string accountName, User user);
        bool IsOwnedByUser(BankAccount account, User user);
        IQueryable<BankAccount> GetByUser(int userId);
        IQueryable<BankAccount> GetByUser(string userEmail);
        int Create(string accountName, string currency, int userId);
        int Create(string accountName, string currency, string userEmail);
    }
}
