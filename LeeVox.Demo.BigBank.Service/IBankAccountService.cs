using System.Collections.Generic;
using System.Linq;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public interface IBankAccountService : IService
    {
        BankAccount Get(string accountName);
        IQueryable<BankAccount> GetByUser(int userId);
        IQueryable<BankAccount> GetByUser(string userEmail);
        int Create(string accountName, string currency, int userId);
        int Create(string accountName, string currency, string userEmail);

        int Deposit(User bankOfficer, string account, string currency, decimal amount, string message);
    }
}
