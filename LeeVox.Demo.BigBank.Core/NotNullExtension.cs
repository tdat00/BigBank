using System;

namespace LeeVox.Demo.BigBank.Core
{
    public static class NotNullExtension
    {
        public static T EnsureNotNull<T>(this T value, string objectName)
        {
            if (Object.ReferenceEquals(null, value))
            {
                throw new ArgumentException($"{objectName} is null.");
            }
            return value;
        }
        public static string EnsureNotNullOrWhiteSpace(this string value, string objectName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{objectName} is null or whitespaces.");
            }
            return value;
        }
    }
}
