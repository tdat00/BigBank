using System;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LeeVox.Demo.BigBank.Service
{
    public class JwtService : IJwtService
    {
        public ILogger<IJwtService> Logger {get; set;}
        public IJwtSessionService JwtSessionService {get; set;}
        private JwtConfig JwtConfig {get; set;}

        public JwtService(IOptions<JwtConfig> jwtConfig, IJwtSessionService jwtSessionService, ILogger<IJwtService> logger)
        {
            this.JwtConfig = jwtConfig.Value;
            this.JwtSessionService = jwtSessionService;
            this.Logger = logger;
        }

        public string GenerateToken(User user)
        {
            var token = string.Empty;
            var random = new CryptoRandom();
            var session = random.RandomBytes(16).GetHexaString();
            var claim = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("first_name", user.FirstName),
                new Claim("last_name", user.LastName),
                new Claim("email", user.Email),
                new Claim("session", session)
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
            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            JwtSessionService.Add(session, jwtToken);
            return token;
        }

        public void RemoveSession(string session)
        {
            JwtSessionService.Remove(session);
        }
    }
}
