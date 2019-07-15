using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ventura.Exceptions;
using Ventura.Interfaces;

using static Ventura.Constants;

namespace Ventura.Accumulator
{
	internal class VenturaAccumulator : IAccumulator
    {
        private readonly IEnumerable<IEntropyExtractor> entropyExtractors;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entropyExtractors"></param>
		/// <param name="token"></param>
        public VenturaAccumulator(IEnumerable<IEntropyExtractor> entropyExtractors, CancellationToken token = default)
        {
            if (entropyExtractors.Count() > MaximumNumberOfSources)
                throw new ArgumentException($"Cannot use more than {MaximumNumberOfSources} sources");

            this.entropyExtractors = entropyExtractors;

            InitializePools();
			RegisterExtractorEvents();
			AccumulateEntropy(token);
        }

        protected List<EntropyPool> Pools { get; private set; }

		/// <summary>
		/// The accumulator has enough collected entropy as soon as first pool is full
		/// </summary>
		public bool HasEnoughEntropy => Pools.First().HasEnoughEntropy;

        /// <summary>
        /// Retrieves entropic data from pools, each pool
        /// is used according to the divisor test 2i % ressed counter.
        /// </summary>
		public byte[] GetRandomDataFromPools(int reseedCounter)
        {
            if (!HasEnoughEntropy)
                throw new AccumulatorEntropyException("Not enough entropy accumulated");

			var randomData = new byte[MaximumSeedSize]; 
			var tempIndex = 0;

			for (int i = 0; i < Pools.Count; i++)
			{
				if (reseedCounter % Math.Pow(2, i) != 0)
					continue; // TODO: investigate if this cna be changed to break for performance

				var data = Pools[i].ReadData();
				Pools[i].Clear();

				data.CopyTo(randomData, tempIndex);
				tempIndex += data.Length;
			}

			return randomData;
        }

		#region Private implementation

		private void InitializePools()
		{
			Pools = new List<EntropyPool>();

	        for (var i = 0; i < MaximumNumberOfPools; i++)
	        {
		        var pool = new EntropyPool(i);
		        Pools.Add(pool);
	        }
        }

        private void RegisterExtractorEvents()
        {
	        foreach (var en in entropyExtractors)
	        {
		        en.OnEntropyAvailable += DistributeEvent;
	        }
        }

		/// <summary>
		/// Starts each entropy extractor on its own thread.
		/// </summary>
        private void AccumulateEntropy(CancellationToken token = default)
        {
	        foreach (var extractor in entropyExtractors)
	        {
		        Task.Factory.StartNew(() =>
		        {
			        while (extractor.IsHealthy)
			        {
				        token.ThrowIfCancellationRequested();
				        extractor.Run();
			        }

		        }, TaskCreationOptions.LongRunning); 
	        }
        }

		private void DistributeEvent(Event successfulExtraction)
		{ 
	        foreach (var pool in Pools)
	        {
		        pool.AddEventData(successfulExtraction.SourceNumber, successfulExtraction.Data);
				//Debug.WriteLine($"Event from source { successfulExtraction.SourceNumber } added from thread: { Thread.CurrentThread.ManagedThreadId }");
	        }
        }

		#endregion

		#region IDisposable

		public void Dispose()
        {
	        foreach (var en in entropyExtractors)
	        {
		        en.OnEntropyAvailable -= DistributeEvent;
	        }
        }

		#endregion
	}
}