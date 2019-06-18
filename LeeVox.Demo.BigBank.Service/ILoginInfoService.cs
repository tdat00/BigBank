using System.Collections.Generic;
using System.Threading.Tasks;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public interface ILoginInfoService: ICacheService<string, LoginInfo>
    {
        string AddLoginInfo(User user);
        string AddLoginInfo(User user, string tokenId);
        void RemoveLoginInfo(string token);
        void RemoveLoginInfo(User user);
    }
}
