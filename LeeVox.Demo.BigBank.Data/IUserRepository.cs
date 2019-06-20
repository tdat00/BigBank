using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Data
{
    public interface IUserRepository : IRepository<User>
    {
        User ByEmail(string email);
    }
}
