using System.Collections.Generic;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public interface IBankAccountService : IService
    {
        BankAccount Get(string accountName);
        int Create(string accountName, string currency, int userId);
        int Create(string accountName, string currency, string userEmail);
    }
}
