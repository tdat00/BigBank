using System.Text;
using System.Security.Cryptography;
using System;

namespace LeeVox.Demo.BigBank.Core
{
    public interface ICryptoHash
    {
        byte[] Sha256(string text);
        byte[] Sha256(string text, Encoding encoding);
        byte[] Sha256(byte[] data);

        byte[] Sha512(string text);
        byte[] Sha512(string text, Encoding encoding);
        byte[] Sha512(byte[] data);

        (byte[] hash, byte[] salt) Pbkdf2Hash(string text);
        (byte[] hash, byte[] salt) Pbkdf2Hash(string text, Encoding encoding);
        byte[] Pbkdf2Hash(string text, byte[] salt);
        byte[] Pbkdf2Hash(string text, byte[] salt, Encoding encoding);
        (byte[] hash, byte[] salt) Pbkdf2Hash(byte[] data);
        byte[] Pbkdf2Hash(byte[] data, byte[] salt);
    }
    public class CryptoHash : ICryptoHash
    {
        public const int RecommendHashSize = 256; // in bit
        public const int RecommendSaltSize = 128; // in bit
        public const int RecommendPbkdf2Iteration = 100_000;

        public ICryptoRandom CryptoRandom {get; set;}
        public CryptoHash(ICryptoRandom cryptoRandom)
        {
            this.CryptoRandom = cryptoRandom;
        }

        public byte[] Sha256(string text)
            => Sha256(Encoding.UTF8.GetBytes(text));
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
            => Sha512(Encoding.UTF8.GetBytes(text));
        public byte[] Sha512(string text, Encoding encoding)
            => Sha512(encoding.GetBytes(text));
        public byte[] Sha512(byte[] data)
        {
            using (var provider = new SHA512CryptoServiceProvider())
            {
                return provider.ComputeHash(data);
            }
        }

        public (byte[] hash, byte[] salt) Pbkdf2Hash(string text)
            => Pbkdf2Hash(Encoding.UTF8.GetBytes(text));
        public (byte[] hash, byte[] salt) Pbkdf2Hash(string text, Encoding encoding)
            => Pbkdf2Hash(encoding.GetBytes(text));
        public byte[] Pbkdf2Hash(string text, byte[] salt)
            => Pbkdf2Hash(Encoding.UTF8.GetBytes(text), salt);
        public byte[] Pbkdf2Hash(string text, byte[] salt, Encoding encoding)
            => Pbkdf2Hash(encoding.GetBytes(text), salt);
        public (byte[] hash, byte[] salt) Pbkdf2Hash(byte[] data)
        {
            var salt = CryptoRandom.RandomBytes(RecommendSaltSize / sizeof(byte));
            var hash = Pbkdf2Hash(data, salt);
            return (hash, salt);
        }
        public byte[] Pbkdf2Hash(byte[] data, byte[] salt)
        {
            using (var provider = new Rfc2898DeriveBytes(data, salt, RecommendPbkdf2Iteration))
            {
                return provider.GetBytes(RecommendHashSize / sizeof(byte));
            }
        }
    }
}
