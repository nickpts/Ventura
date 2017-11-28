using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Ventura.Exceptions
{
    [Serializable]
    public class GeneratorOutputException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public GeneratorOutputException()
        {
        }

        public GeneratorOutputException(string message) : base(message)
        {
        }

        public GeneratorOutputException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
