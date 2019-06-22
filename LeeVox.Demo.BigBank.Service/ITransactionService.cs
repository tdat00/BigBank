using System.Collections.Generic;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public interface ITransactionService : IService
    {
        int Deposit(User bankOfficer, string account, string currency, decimal money, string message);
        int Withdraw(string account, decimal money, string message);
    }
}
