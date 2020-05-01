using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using static Ventura.Constants;

namespace Ventura.Accumulator
{
	/// <summary>
	/// Holds events large enough that an attacker can no longer enumerate possible values for events
	/// Each random event is appended to the existing hashed event and a new hash is computed. This is
	/// an explicit design decision to avoid having a pool hold too much event data- the spec itself does
	/// not specify the size of the pool.
	/// </summary>
	internal sealed class EntropyPool
	{
		private byte[] hash = new byte[MinimumPoolSize];
		private readonly object syncRoot;
		private int eventsStored = 0;
		private long runningSize;

		public EntropyPool(int poolNumber)
		{
			PoolNumber = poolNumber;
			syncRoot = new object();
		}

		public int PoolNumber { get; }

		public void AddEventData(int sourceNumber, byte[] data)
		{
			if (sourceNumber < 0 || sourceNumber > MaximumNumberOfSources)
				throw new ArgumentException($"{ nameof(sourceNumber) } must be between 0 and { MaximumNumberOfSources }");

			if (data.Length > MaximumEventSize)
				throw new ArgumentException($"{ nameof(data.Length) } cannot be more than { MaximumEventSize } bytes");

			lock (syncRoot)
			{
				var concatenatedData = hash.Concat(data).ToArray();

				using (var algorithm = SHA256.Create())
				{
					hash = algorithm.ComputeHash(concatenatedData); 
				}
				
				Interlocked.Add(ref runningSize, concatenatedData.Length);
				Interlocked.Increment(ref eventsStored);

#if DEBUG
				if (PoolNumber == 0 && eventsStored % 10000 == 0)
					Debug.WriteLine($"Pool {PoolNumber} contains entropy of {eventsStored} events");
#endif
			}
		}

		public bool HasEnoughEntropy => Interlocked.Read(ref runningSize) >= MinimumPoolSize;

		public byte[] ReadData()
		{
			lock (syncRoot)
			{
				return hash;
			}
		}

		public void Clear()
		{
			Interlocked.Exchange(ref runningSize, 0);

			lock (syncRoot)
			{
				Array.Clear(hash, 0, hash.Length);
			}
		}
	}
}
