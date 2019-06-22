using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using static Ventura.Constants;

namespace Ventura.Accumulator
{
	internal sealed class EntropyPool: IDisposable
	{
		private byte[] hash = new byte[MinimumPoolSize];
		private readonly SHA256 hashAlgorithm;
		private readonly object syncRoot;
		private int eventsStored = 0;
		private long runningSize;
		private int poolNumber;

		public EntropyPool(int poolNumber)
		{
			this.poolNumber = poolNumber;
			syncRoot = new object();
			hashAlgorithm = SHA256.Create();
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
				hash = hashAlgorithm.ComputeHash(concatenatedData);
				Interlocked.Add(ref runningSize, concatenatedData.Length);
				Interlocked.Increment(ref eventsStored);
			}
		}

		public bool HasEnoughEntropy => Interlocked.Read(ref runningSize) >= MinimumPoolSize;

		public byte[] ReadData() => hash;

		public void Clear()
		{
			Interlocked.Exchange(ref runningSize, 0);
			Array.Clear(hash, 0, hash.Length);
		}

		public void Dispose()
		{
			hashAlgorithm.Dispose();
		}
	}
}
