using System;

using static Ventura.Constants;

namespace Ventura.Generator
{
    public struct VenturaPrngState
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
