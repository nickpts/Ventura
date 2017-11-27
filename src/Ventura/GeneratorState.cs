using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura
{
    public class GeneratorState
    {
        public int Counter { get; internal set; }
        public byte[] Seed { get; internal set; }
        public bool Seeded { get; internal set; }
    }
}
