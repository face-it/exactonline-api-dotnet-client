using System;

namespace ExactOnline.Client.Sdk.Exceptions
{
    public class TooManyRequestsException : Exception
    {
        public TooManyRequestsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
