using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Data
{
    public interface ICurrencyRepository : IRepository<Currency>
    {
        Currency ByName(string name);
    }
}
