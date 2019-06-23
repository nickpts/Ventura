using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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

            InitializePools();
			RegisterExtractorEvents();
			AccumulateEntropy();
        }

		public bool HasEnoughEntropy => pools.First().HasEnoughEntropy;
		
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

		#region Private implementation

		private void InitializePools()
        {
	        for (var i = 0; i < MaximumNumberOfPools; i++)
	        {
		        var pool = new EntropyPool(i);
		        pools.Add(pool);
	        }
        }

        private void RegisterExtractorEvents()
        {
	        foreach (var ex in entropyExtractors)
	        {
		        ex.EntropyAvailable += OnEntropyAvailable;
	        }
        }

        private void AccumulateEntropy()
        {
	        foreach (var extractor in entropyExtractors)
	        {
		        Task.Factory.StartNew(() =>
		        {
			        while (true)
			        {
				        extractor.Run();
			        }

		        }, TaskCreationOptions.LongRunning); //TODO: canxellation token?

			}
        }

		private void OnEntropyAvailable(Event successfulExtraction)
        {
	        foreach (var pool in pools)
	        {
		        pool.AddEventData(successfulExtraction.SourceNumber, successfulExtraction.Data);
				Debug.WriteLine($"Event from source { successfulExtraction.SourceNumber } added from thread: { Thread.CurrentThread.ManagedThreadId }");
	        }
        }

		#endregion

		#region IDisposable

		public void Dispose()
        {
	        foreach (var ex in entropyExtractors)
	        {
		        ex.EntropyAvailable -= OnEntropyAvailable;
	        }

	        foreach (var p in pools)
	        {
				p.Dispose();
	        }
        }

		#endregion
	}
}