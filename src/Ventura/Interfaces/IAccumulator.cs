using System;

namespace Ventura.Interfaces
{
	/// <summary>
	/// Collects real random data from various sources and uses it to reseed the generator
	/// </summary>
	interface IAccumulator : IDisposable
    {
	    bool HasEnoughEntropy { get; }

		byte[] GetRandomDataFromPools(int reseedCounter);

        void Dispose();
    }
}
