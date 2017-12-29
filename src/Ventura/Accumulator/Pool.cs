using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Ventura.Accumulator
{
    public class Pool
    {
        private int poolNumber;
        private readonly List<byte> hash = new List<byte>();

        public Pool(int poolNumber) => this.poolNumber = poolNumber;
        
        public void Process(IEnumerable<Event> events)
        {
            var data = events.SelectMany(e => e.Data).ToArray();
            var hashedData = SHA256.Create().ComputeHash(data);
            hash.AddRange(hashedData);
        }

        public byte[] HashedData => hash.ToArray();
    }
}
