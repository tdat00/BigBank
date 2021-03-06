using System;
using System.Collections.Generic;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Linq;

namespace LeeVox.Demo.BigBank.Service
{
    public class JwtService : IJwtService
    {
        public ILogger<IJwtService> Logger {get; set;}
        public IJwtSessionService JwtSessionService {get; set;}
        private JwtConfig JwtConfig {get; set;}

        public JwtService(
            IOptions<JwtConfig> jwtConfig,
            IJwtSessionService jwtSessionService,
            ILogger<IJwtService> logger
        )
        {
            this.JwtConfig = jwtConfig.Value;
            this.JwtSessionService = jwtSessionService;
            this.Logger = logger;
        }

        public string GenerateToken(User user)
        {
            user.EnsureNotNull(nameof(user));

            var random = new CryptoRandom();
            var session = random.RandomBytes(16).GetBase64String();
            var roles = user.Role.ToString().Split(',').Select(x => new Claim(ClaimTypes.Role, x.Trim()));
            var claim = new List<Claim>(roles)
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, session)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                
            var jwtToken = new JwtSecurityToken(
                JwtConfig.Issuer,
                JwtConfig.Audience,
                claim,
                expires: DateTime.Now.AddMinutes(JwtConfig.AccessExpiration),
                signingCredentials: credentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            JwtSessionService.Add(session, jwtToken);
            return token;
        }

        public void RemoveSession(string session)
        {
            session.EnsureNotNullOrWhiteSpace(nameof(session));

            JwtSessionService.Remove(session);
        }
    }
}
