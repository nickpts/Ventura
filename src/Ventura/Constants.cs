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

        public static int BlockKeySize = 256;
        public static double MaximumRequestSize = Math.Pow(2, 20);
    }
}
