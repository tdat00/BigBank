using System.Collections.Generic;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public interface IBankAccountService : IService
    {
        int Create(BankAccount account);
    }
}
