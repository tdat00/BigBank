using System;

namespace LeeVox.Demo.BigBank.Core
{
    public static class NotNullExtension
    {
        public static T EnsureNotNull<T>(this T value, string objectName = null, string message = null)
        {
            if (Object.ReferenceEquals(null, value))
            {
                throw new ArgumentException(message ?? $"{objectName ?? "Object"} is null.");
            }
            return value;
        }
        public static string EnsureNotNullOrWhiteSpace(this string value, string objectName = null, string message = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(message ?? $"{objectName ?? "Object"} is null or whitespaces.");
            }
            return value;
        }
    }
}
