using System;
using System.Security.Cryptography;

namespace LeeVox.Demo.BigBank.Core
{
    public interface ICryptoRandom
    {
        byte[] RandomBytes(int length);
    }

    public class CryptoRandom : ICryptoRandom
    {
        public byte[] RandomBytes(int length)
        {
            using (var provider = new RNGCryptoServiceProvider())
            {
                var result = new byte[length];
                provider.GetBytes(result);
                return result;
            }
        }
    }
}
