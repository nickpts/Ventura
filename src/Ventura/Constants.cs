using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura
{
    public static class Constants
    {
        public enum Cipher
        {
            Aes,
            TwoFish
        }

        public const int BlockKeySize = 256;
        public const double MaximumRequestSize = 1048576;
        
    }
}
