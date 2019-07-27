using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly Random randomPoolPicker;

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
            this.randomPoolPicker = new Random();

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
				EmptyPools(i);

				data.CopyTo(randomData, tempIndex);
				tempIndex += data.Length;
			}

			return randomData;
        }

		#region Private implementation

		/// <summary>
		/// Instead of emptying only pool i as described in Fortuna, pool j with
		/// j <= i is emptied instead. This potentially leads
		/// to improved results in the face of adversarial starting times between
		/// the emptying of pools (Dodis et al)
		/// </summary>
		private void EmptyPools(int i)
		{
			var poolsToEmpty = Pools.Where(p => p.PoolNumber <= i);

			foreach (var pool in poolsToEmpty)
			{
				pool.Clear();
			}
		}

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

		/// <summary>
		/// The pool to be filled is chosen pseudorandomly as recommended by Dodis et al, as
		/// opposed to the round-robin fashion Fortuna uses. In theory, this provides slightly
		/// better performance against constant sequence samplers than against arbitrary adversaries.
		/// While better than round-robin, ideally as pseudorandom permutation of all P pools every P steps
		/// should be generated and a pool filled via a modulo operation as recommended in Dodis et al 6.3
		/// </summary>
		private void DistributeEvent(Event successfulExtraction)
		{
			var poolToFill = Pools[randomPoolPicker.Next(0, MaximumNumberOfPools - 1)];

		    poolToFill.AddEventData(successfulExtraction.SourceNumber, successfulExtraction.Data);

			Debug.WriteLine($"Event from source { successfulExtraction.SourceNumber } " +
			                $"added from thread: { Thread.CurrentThread.ManagedThreadId } " +
			                $"to pool { poolToFill.PoolNumber }");
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