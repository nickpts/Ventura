using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ventura.Interfaces;

namespace Ventura.Accumulator
{
    /// <summary>
    /// Collects real random data from various sources
    /// and uses it to reseed the generator
    /// </summary>
    public class VenturaAccumulator: IAccumulator
    {
        private IEnumerable<IEntropyExtractor> entropyExtractors;
        private List<Pool> pools;

        public VenturaAccumulator(IEnumerable<IEntropyExtractor> entropyExtractors)
        {
            this.entropyExtractors = entropyExtractors;
            pools = new List<Pool>();
        }

        public void InitalisePools()
        {
            for (int i = 0; i < 32; i++)
            {
                var pool = new Pool(i);
                pools.Add(pool);
            }
        }

        public void InitialiseExtractors()
        {
            
        }
    }
}
