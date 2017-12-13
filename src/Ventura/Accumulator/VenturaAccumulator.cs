using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ventura.Interfaces;
using static Ventura.Constants;

namespace Ventura.Accumulator
{
    /// <summary>
    /// Collects real random data from various sources
    /// and uses it to reseed the generator
    /// </summary>
    public class VenturaAccumulator: IAccumulator
    {
        private IEnumerable<IEntropyExtractor> entropyExtractors;
        private List<Pool> pools = new List<Pool>();

        public VenturaAccumulator(IEnumerable<IEntropyExtractor> entropyExtractors)
        {
            this.entropyExtractors = entropyExtractors;
            InitalisePools();
        }

        public void InitalisePools()
        {
            for (int i = 0; i < MaximumNumberOfPools; i++)
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
