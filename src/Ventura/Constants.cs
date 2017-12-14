using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura
{
    public class Constants
    {
        public const int KeyBlockSize = 32; // 256 bits
        public const int CipherBlockSize = 16; // 128 bits
        public const int MaximumRequestSize = 1048576;
        public const int NumberOfPools = 32;
        public const int MaximumNumberOfSources = 255;
        public const int MaximumNumberOfPools = 32;
    }
}
