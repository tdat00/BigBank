using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Data
{
    public interface IBankAccountRepository : IRepository<BankAccount>
    {
        BankAccount ByAccountNumber(string accountNumber);
    }
}
