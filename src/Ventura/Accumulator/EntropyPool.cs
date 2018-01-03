using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using static Ventura.Constants;

namespace Ventura.Accumulator
{
    public class EntropyPool
    {
        private int poolNumber;
        private readonly List<byte> hash = new List<byte>();

        public EntropyPool(int poolNumber) => this.poolNumber = poolNumber;

        public void AddEventData(int sourceNumber, byte[] data)
        {
            if (sourceNumber < 0 || sourceNumber > MaximumNumberOfSources) throw new ArgumentException($"{ nameof(sourceNumber) } must be between 0 and { MaximumNumberOfSources }");
            if (data.Length > MaximumEventSize) throw new ArgumentException($"{ nameof(data.Length) } cannot be more than { MaximumEventSize } bytes");

            var hashedData = SHA256.Create().ComputeHash(data);
            hash.AddRange(hashedData);
        }

        public byte[] HashedData => hash.ToArray();
        public bool HasEnoughEntropy => HashedData.Length >= MinimumPoolSize;
    }
}
