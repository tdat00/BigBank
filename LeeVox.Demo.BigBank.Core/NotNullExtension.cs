using System;

namespace LeeVox.Demo.BigBank.Core
{
    public static class NotNullExtension
    {
        public static T EnsureNotNull<T>(this T value, string paramName)
        {
            if (Object.ReferenceEquals(null, value))
            {
                throw new ArgumentException($"{paramName} should not be null.", paramName);
            }
            return value;
        }
        public static string EnsureNotNullOrWhiteSpace(this string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{paramName} should not be null.", paramName);
            }
            return value;
        }
    }
}
