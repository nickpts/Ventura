using System;

namespace Ventura.Exceptions
{
    [Serializable]
    public class EntropyEventFailedException : Exception
    {
        public EntropyEventFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
