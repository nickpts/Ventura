using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Interfaces
{
    public interface IVenturaService
    {
        void InitialisePRNG();
        byte[] GetRandomData();
    }
}
