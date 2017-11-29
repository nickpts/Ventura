using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Ventura.Exceptions
{
    [Serializable]
    public class GeneratorInputException : Exception
    {
        public int Size { get; protected set; }

        public GeneratorInputException(string message)
            :base($"Invalid size for input array: { message }")
        {

        }
    }
}
