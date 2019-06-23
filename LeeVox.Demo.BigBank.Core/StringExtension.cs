using System;

namespace LeeVox.Demo.BigBank.Core
{
    public static class StringExtension
    {
        public static string GetHexaString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public static string GetBase64String(this byte[] bytes)
            => Convert.ToBase64String(bytes);

        public static byte[] ParseBase64String(this string base64String)
            => Convert.FromBase64String(base64String);
    }
}
