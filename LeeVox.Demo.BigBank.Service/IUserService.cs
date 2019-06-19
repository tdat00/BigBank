using System.Collections.Generic;
using System.Threading.Tasks;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public interface IUserService
    {
        User Get(int id);
        User Get(string email);
        IEnumerable<User> Get(string search = null, int skip = 0, int take = 10);

        int Create(User customer);
        string Login(string email, string password);
        void Logout(string email);
        void Delete(int id);
        void Delete(string email);
    }
}
