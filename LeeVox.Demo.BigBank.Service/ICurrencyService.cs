using System.Collections.Generic;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public interface ICurrencyService : IService
    {
        Currency Get(int id);
        Currency Get(string name);
    }
}
