using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public interface IJwtService : IService
    {
        string GenerateToken(User user);
        void RemoveSession(string session);
    }
}
