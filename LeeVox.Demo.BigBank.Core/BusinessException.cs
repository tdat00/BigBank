using System;

namespace LeeVox.Demo.BigBank.Core
{
    public class BusinessException : Exception
    {
        public BusinessException(string message)
            : base(message)
        {
            
        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
    }
}
