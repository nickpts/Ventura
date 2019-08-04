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
		private readonly IEventEmitter eventEmitter;

		/// <summary>
		/// Event handler for successful entropy events
		/// </summary>
		public event EntropyAvailabilityHandler OnEntropyAvailable;

		protected EntropyExtractorBase(IEventEmitter eventEmitter)
		{
			this.eventEmitter = eventEmitter;
			SourceNumber = eventEmitter.SourceNumber;
			IsHealthy = true;
		}

		/// <summary>
		/// Running total of failed event operations
		/// </summary>
		public int FailedEventCount { get; protected set; }

		/// <summary>
		/// Indicates if an extractor has been consistently producing successful events
		/// </summary>
		public bool IsHealthy{ get; protected set; }

		public int SourceNumber { get; }
		public virtual string SourceName { get; protected set; }

		public virtual Task Run()
		{
			var result = eventEmitter.Execute(GetEntropicData()).Result;

			if (result.ExtractionSuccessful)
				OnEntropyAvailable?.Invoke(result);
			else
			{
				FailedEventCount++;

				if (FailedEventCount >= Constants.FailedEventThreshold)
					IsHealthy = false;
			}

			return Task.CompletedTask;
		}

		public abstract Func<byte[]> GetEntropicData();
	}
}