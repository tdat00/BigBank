using System;
using System.Text;
using System.Security.Cryptography;

namespace LeeVox.Demo.BigBank.Core
{
    public class CryptoHash
    {
        public byte[] Sha256(string text)
            => Sha256(text, Encoding.UTF8);
        public byte[] Sha256(string text, Encoding encoding)
            => Sha256(encoding.GetBytes(text));
        public byte[] Sha256(byte[] data)
        {
            using (var provider = new SHA256CryptoServiceProvider())
            {
                return provider.ComputeHash(data);
            }
        }

        public byte[] Sha512(string text)
            => Sha512(text, Encoding.UTF8);
        public byte[] Sha512(string text, Encoding encoding)
            => Sha512(encoding.GetBytes(text));
        public byte[] Sha512(byte[] data)
        {
            using (var provider = new SHA512CryptoServiceProvider())
            {
                return provider.ComputeHash(data);
            }
        }
    }
}
