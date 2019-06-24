using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
					continue;

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
	        foreach (var ex in entropyExtractors)
	        {
		        ex.EntropyAvailable += OnEntropyAvailable;
	        }
        }

		/// <summary>
		/// Starts each entropy extractor on its own thread.
		/// The extractor will continue emitting events until cancel is called on the token
		/// or the prng is stopped.
		/// </summary>
        private void AccumulateEntropy(CancellationToken token = default(CancellationToken))
        {
	        foreach (var extractor in entropyExtractors)
	        {
		        Task.Factory.StartNew(() =>
		        {
			        while (true)
			        {
				        if (token.IsCancellationRequested)
					        break;

				        extractor.Run();
			        }

		        }, TaskCreationOptions.LongRunning); //TODO: cancellation token, but what happens on restart?
	        }
        }

		private void OnEntropyAvailable(Event successfulExtraction)
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
	        foreach (var ex in entropyExtractors)
	        {
		        ex.EntropyAvailable -= OnEntropyAvailable;
	        }

	        foreach (var p in Pools)
	        {
				p.Dispose();
	        }
        }

		#endregion
	}
}