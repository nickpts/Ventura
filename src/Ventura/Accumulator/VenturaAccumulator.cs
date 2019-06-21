using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Ventura.Interfaces;

using static Ventura.Constants;

namespace Ventura.Accumulator
{
    /// <summary>
    /// Collects real random data from various sources
    /// and uses it to reseed the generator
    /// </summary>
    internal class VenturaAccumulator : IAccumulator
    {
        private readonly IEnumerable<IEntropyExtractor> entropyExtractors;
        private readonly List<EntropyPool> pools = new List<EntropyPool>();

        public VenturaAccumulator(IEnumerable<IEntropyExtractor> entropyExtractors)
        {
            if (entropyExtractors.Count() > MaximumNumberOfSources)
                throw new ArgumentException($"Cannot use more than {MaximumNumberOfSources} sources");

            this.entropyExtractors = entropyExtractors;

            for (var i = 0; i < MaximumNumberOfPools; i++)
            {
                var pool = new EntropyPool(i);
                pools.Add(pool);
            }
        }

        public bool HasEnoughEntropy => pools.First().HasEnoughEntropy;
        
        public void Distribute()
        {

	        foreach (var ex in entropyExtractors)
	        {
				ex.Start();
	        }


	        //foreach (var pool in pools)
         //   foreach (var extractor in entropyExtractors)
         //   {
         //       extractor.Start();

         //       pool.AddEventData(
         //           extractor.SourceNumber,
         //           extractor.Events.Last(e => e.ExtractionSuccessful).Data.ToArray());

         //       Debug.WriteLine($"Extractor {extractor.SourceNumber}: {extractor.SourceName} has " +
         //                       $"{extractor.Events.Where(c => c.ExtractionSuccessful).SelectMany(c => c.Data).ToArray().Length } bytes of entropy");
         //   }
        }

        public byte[] GetRandomDataFromPools(int reseedCounter)
        {
            if (!HasEnoughEntropy)
                throw new InvalidOperationException("Not enough entropy accumulated");

			var randomData = new byte[MaximumSeedSize];
			var tempIndex = 0;

			for (int i = 0; i < pools.Count; i++)
			{
				if (Math.Pow(2, i) % reseedCounter != 0)
					continue;

				var data = pools[i].ReadData();
				pools[i].Clear();

				data.CopyTo(randomData, tempIndex);
				tempIndex += data.Length;
			}

			return randomData;
        }
    }
}