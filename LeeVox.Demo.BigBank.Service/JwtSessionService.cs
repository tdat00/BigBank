using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace LeeVox.Demo.BigBank.Service
{
    //TODO: inherite InMemoryCache for thread-safety.
    public class JwtSessionService : Dictionary<string, JwtSecurityToken>, IJwtSessionService
    {
    }
}
