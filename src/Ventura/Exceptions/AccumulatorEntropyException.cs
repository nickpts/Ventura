using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Exceptions
{
    public class AccumulatorEntropyException: Exception
    {
        public AccumulatorEntropyException(string message)
            : base(message)
        {
        }
    }
}
