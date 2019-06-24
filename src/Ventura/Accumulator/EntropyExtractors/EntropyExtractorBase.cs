using System;
using System.Threading.Tasks;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors
{
	/// <summary>
	/// Base class for entropy extractors, collects real random data from sources.
	/// </summary>
	public abstract class EntropyExtractorBase 
	{
		/// <summary>
		/// Runs extraction logic
		/// </summary>
		private readonly EventEmitter eventEmitter;
		public event EntropyAvailabilityHandler EntropyAvailable;

		protected EntropyExtractorBase(int sourceNumber)
		{
			SourceNumber = sourceNumber;
			eventEmitter = new EventEmitter(sourceNumber);
		}

		public virtual string SourceName { get; protected set; }
		public int SourceNumber { get; }

		public virtual Task Run()
		{
			var result = eventEmitter.Execute(ExtractEntropicData()).Result;

			if (result.ExtractionSuccessful) 
				EntropyAvailable?.Invoke(result);

			return Task.CompletedTask;
		}

		protected abstract Func<byte[]> ExtractEntropicData();
	}
}