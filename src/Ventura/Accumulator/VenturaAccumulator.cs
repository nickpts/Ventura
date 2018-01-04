using System;
using System.Collections.Generic;
using System.Linq;

using Ventura.Interfaces;
using static Ventura.Constants;

namespace Ventura.Accumulator
{
    /// <summary>
    /// Collects real random data from various sources
    /// and uses it to reseed the generator
    /// </summary>
    internal class VenturaAccumulator: IAccumulator
    {
        private readonly IEnumerable<IEntropyExtractor> entropyExtractors;
        private readonly List<EntropyPool> pools = new List<EntropyPool>();

        public VenturaAccumulator(IEnumerable<IEntropyExtractor> entropyExtractors)
        {
            if (entropyExtractors.Count() > MaximumNumberOfSources)
                throw new ArgumentException($"Cannot use more than {MaximumNumberOfSources} sources");
                    
            this.entropyExtractors = entropyExtractors;
        }
        
        public void Collect()
        {
            InitialisePools();
            StartExtractors();
            Distribute();
        }

        #region Private implementation

        private void InitialisePools()
        {
            for (int i = 0; i < MaximumNumberOfPools; i++)
            {
                var pool = new EntropyPool(i);
                pools.Add(pool);
            }
        }

        private void StartExtractors()
        {
            foreach (var extractor in entropyExtractors)
            {
                extractor.Start();
            }
        }

        private void Distribute()
        {
            foreach (var pool in pools)
            {
                foreach (var extractor in entropyExtractors)
                {
                    pool.AddEventData(
                        extractor.SourceNumber,
                        extractor.Events.Where(e => e.ExtractionSuccessful).SelectMany(e => e.Data).ToArray());
                }
            }
        }

        #endregion
    }
}