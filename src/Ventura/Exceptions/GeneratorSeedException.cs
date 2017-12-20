using System;

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