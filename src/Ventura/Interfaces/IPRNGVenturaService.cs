using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Interfaces
{
    public interface IPRNGVenturaService
    {
        void InitialisePRNG();
        byte[] GetRandomData();
    }
}
