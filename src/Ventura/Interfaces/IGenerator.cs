using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Interfaces
{
    public interface IGenerator
    {
        byte[] GenerateData(byte[] input);
        void Reseed(byte[] seed);
    }
}
