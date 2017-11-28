using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura
{
    public interface IGenerator
    {
        byte[] GenerateRandomData(byte[] input);
        byte[] Reseed(byte[] seed);
    }
}
