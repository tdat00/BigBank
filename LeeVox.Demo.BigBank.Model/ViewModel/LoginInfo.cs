using System;

namespace LeeVox.Demo.BigBank.Model
{
    public class LoginInfo
    {
        public string TokenId {get; set;}
        public User User {get; set;}
        public DateTime LoggedOnUtc {get; set;}
        public DateTime ExpiredOnUtc {get; set;}
    }
}
