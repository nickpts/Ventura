using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Exceptions
{
    public class EntropyEventFailedException : Exception
    {
        public EntropyEventFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
