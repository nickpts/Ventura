using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Generator
{
    internal class VenturaPrngState
    {
        public int Counter { get; internal set; }
        public byte[] Key { get; internal set; }
        public bool Seeded { get; internal set; }

        /// <summary>
        /// Transforms the counter to a byte array. 
        /// Intended to be used on
        /// </summary>
        public byte[] TransformCounterToByteArray()
        {
            var destArray = new byte[16];
            var sourceArray = BitConverter.GetBytes(Counter);

            Array.Copy(sourceArray, destArray, sourceArray.Length);

            return destArray;
        }
    }
}
