using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeeVox.Sdk;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Model;
using System.Collections;

namespace LeeVox.Demo.BigBank.Service
{
    public class LoginInfoService : Dictionary<string, LoginInfo>, ILoginInfoService
    {
        public string AddLoginInfo(User user)
        {
            var random = new CryptoRandom();
            return AddLoginInfo(user, random.RandomBytes(16).GetHexaString());
        }

        public string AddLoginInfo(User user, string tokenId)
        {
            var info = new LoginInfo
            {
                User = user,
                TokenId = tokenId,
                LoggedOnUtc = DateTime.UtcNow,

                //TODO: need to remove from cache after some time
                ExpiredOnUtc = DateTime.MaxValue.ToUniversalTime()
            };
            this.Add(tokenId, info);
            return tokenId;
        }

        public void RemoveLoginInfo(string token)
        {
            this.Remove(token);
        }

        public void RemoveLoginInfo(User user)
        {
            foreach (var value in this.Values)
            {
                if (user.Email.IsOrdinalEqual(value.User.Email, true))
                {
                    this.Remove(value.TokenId);
                }
            }
        }
    }
}
