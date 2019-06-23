using System;

namespace Ventura.Interfaces
{
    interface IAccumulator: IDisposable
    {
        bool HasEnoughEntropy { get; }
        byte[] GetRandomDataFromPools(int reseedCounter);
        void Dispose();
    }
}
