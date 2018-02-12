using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Interfaces
{
    interface IAccumulator
    {
        bool HasEnoughEntropy { get; }
        void Gather();
    }
}
