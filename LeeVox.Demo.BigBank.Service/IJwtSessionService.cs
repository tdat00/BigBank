using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace LeeVox.Demo.BigBank.Service
{
    public interface IJwtSessionService: IDictionary<string, JwtSecurityToken>, IService
    {
    }
}
