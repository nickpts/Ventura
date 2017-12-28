using System;
using System.Runtime.CompilerServices;

using static Ventura.Constants;

[assembly: InternalsVisibleTo("Ventura.Tests")]
[assembly: InternalsVisibleTo("Ventura.Tests.Visualisation")]
namespace Ventura.Generator
{
    internal class VenturaPrngState
    {
        public int Counter { get; internal set; }
        public byte[] Key { get; internal set; }
        public bool Seeded { get; internal set; }

        public byte[] TransformCounterToByteArray()
        {
            var destArray = new byte[CipherBlockSize];
            var sourceArray = BitConverter.GetBytes(Counter);

            Array.Copy(sourceArray, destArray, sourceArray.Length);

            return destArray;
        }
    }
}
