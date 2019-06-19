using System.Security.Cryptography;

namespace LeeVox.Demo.BigBank.Core
{
    public class CryptoRandom
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
