using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors
{
	public abstract class EntropyExtractorBase : IDisposable
	{
		private readonly EventEmitter eventEmitter;

		public event EntropyAvailabilityHandler EntropyAvailable;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourceNumber"></param>
		protected EntropyExtractorBase(int sourceNumber)
		{
			SourceNumber = sourceNumber;
			eventEmitter = new EventEmitter(sourceNumber);
		}

		public virtual string SourceName { get; protected set; }

		public int SourceNumber { get; }

		public virtual Task Start()
		{
			var result = eventEmitter.Execute(ExtractEntropicData()).Result;
			EntropyAvailable?.Invoke(result);

			return Task.CompletedTask;
		}

		protected abstract Func<byte[]> ExtractEntropicData();

		public void Dispose()
		{

		}
	}
}