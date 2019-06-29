using System;
using System.Collections.Generic;
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
		private readonly int poolNumber;
		private int eventsStored = 0;
		private long runningSize;

		public EntropyPool(int poolNumber)
		{
			this.poolNumber = poolNumber;
			syncRoot = new object();
		}

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

				if (poolNumber == 0 && eventsStored % 10000 == 0)
					Debug.WriteLine($"Pool {poolNumber} contains entropy of {eventsStored} events");
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
