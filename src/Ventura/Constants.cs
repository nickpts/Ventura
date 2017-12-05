using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura
{
    public class Constants
    {
        public enum Cipher
        {
            Aes,
            TwoFish
        }

        public const int KeyBlockSize = 32; // 256 bits
        public const int CipherBlockSize = 16; // 128 bits
        public const int MaximumRequestSize = 1048576;
        
    }
}
