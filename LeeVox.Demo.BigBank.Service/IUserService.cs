using System.Collections.Generic;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public interface IUserService : IService
    {
        User Get(int id);
        User Get(string email);

        IEnumerable<BankAccount> GetBankAccounts(string email);

        int Create(string email, string password, string role, string firstName, string lastName);
        int Create(string email, string password, string role, string firstName, string lastName, string bankAccount, string bankAccountCurrency);
        string Login(string email, string password);
        void Logout(string token);
    }
}
