using System.IdentityModel.Tokens.Jwt;

namespace LeeVox.Demo.BigBank.Service
{
    public interface IJwtSessionService: ICacheService<string, JwtSecurityToken>
    {
    }
}
