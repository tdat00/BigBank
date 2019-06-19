using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace LeeVox.Demo.BigBank.Service
{
    public class JwtSessionService : Dictionary<string, JwtSecurityToken>, IJwtSessionService
    {
    }
}
