using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Ventura.Exceptions
{
    [Serializable]
    public class GeneratorSeedException: Exception
    {
        public GeneratorSeedException(string message) : base(message)
        {
        }
    }
}
