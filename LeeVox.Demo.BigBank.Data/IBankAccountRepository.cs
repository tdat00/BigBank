using System.Linq;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Data
{
    public interface IBankAccountRepository : IRepository<BankAccount>
    {
        BankAccount ByName(string accountName);
        IQueryable<BankAccount> ByUser(int userId);
    }
}
