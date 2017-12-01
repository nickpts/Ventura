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
    }
}
