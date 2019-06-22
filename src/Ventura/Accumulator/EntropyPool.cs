using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using static Ventura.Constants;

namespace Ventura.Accumulator
{
	internal sealed class EntropyPool
	{
		private int poolNumber;
		private byte[] hash = new byte[MinimumPoolSize];
		private int eventsStored = 0;
		private readonly object locker = new object(); // TODO: proper locking!


		private int _runningSize; // TODO: rewrite

		public EntropyPool(int poolNumber) => this.poolNumber = poolNumber;

		public void AddEventData(int sourceNumber, byte[] data)
		{
			if (sourceNumber < 0 || sourceNumber > MaximumNumberOfSources)
				throw new ArgumentException($"{ nameof(sourceNumber) } must be between 0 and { MaximumNumberOfSources }");

			if (data.Length > MaximumEventSize)
				throw new ArgumentException($"{ nameof(data.Length) } cannot be more than { MaximumEventSize } bytes");

			lock (locker)
			{
				var newData = hash.Concat(data).ToArray();
				hash = SHA256.Create().ComputeHash(newData);
				Interlocked.Add(ref _runningSize, newData.Length);
				eventsStored++;
			}
		}

		public bool HasEnoughEntropy => _runningSize >= MinimumPoolSize;

		public byte[] ReadData()
		{
			lock (locker)
			{
				return hash;
			}
		}

		public void Clear()
		{
			lock (locker)
			{
				Interlocked.Exchange(ref _runningSize, 0);
				Array.Clear(hash, 0, hash.Length);
			}
		}
	}
}
