using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using static Ventura.Constants;

namespace Ventura.Accumulator
{
    internal sealed class EntropyPool
    {
        private int poolNumber;
        private readonly List<byte> hash = new List<byte>();
        private int eventsStored = 0;

        public EntropyPool(int poolNumber) => this.poolNumber = poolNumber;

        public void AddEventData(int sourceNumber, byte[] data)
        {
            if (sourceNumber < 0 || sourceNumber > MaximumNumberOfSources)
                throw new ArgumentException($"{ nameof(sourceNumber) } must be between 0 and { MaximumNumberOfSources }");

            if (data.Length > MaximumEventSize)
                throw new ArgumentException($"{ nameof(data.Length) } cannot be more than { MaximumEventSize } bytes");

            var hashedData = SHA256.Create().ComputeHash(data);
            hash.AddRange(hashedData);
            eventsStored++;
        }

        public bool HasEnoughEntropy => hash.Count >= MinimumPoolSize;

		public byte[] ReadData()
        {
	        return hash.ToArray();
        }

        public void Clear()
        {
			hash.Clear();
        }
    }
}
