using System.Collections.Generic;
using System.Threading.Tasks;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public interface ICacheService<TKey, TValue>
    {
        void Add(TKey key, TValue value);
        bool ContainsKey(TKey key);
        bool Remove(TKey key);
    }
}
