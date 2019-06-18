using System;
using System.Collections.Generic;

namespace LeeVox.Demo.BigBank.Core
{
    public static class StringExtension
    {
        public static string GetHexaString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
