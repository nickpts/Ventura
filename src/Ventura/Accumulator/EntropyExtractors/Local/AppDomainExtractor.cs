using System;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors
{
	/// <summary>
	/// Concatenates and returns information regarding the current .NET
	/// application domain's memory and processor metrics.
	/// </summary>
	public class AppDomainExtractor : EntropyExtractorBase, IEntropyExtractor
	{
		public AppDomainExtractor(IEventEmitter eventEmitter) : base(eventEmitter)
		{
		}

		public override string SourceName { get; protected set; } = ".NET Application domain entropy extractor";

		protected override Func<byte[]> ExtractEntropicData()
		{
			return () =>
			{
				AppDomain.MonitoringIsEnabled = true;

				var domain = AppDomain.CurrentDomain;

				var survivedMemory = domain.MonitoringSurvivedMemorySize;
				var totalMemory = domain.MonitoringTotalAllocatedMemorySize;
				var totalProcessorTime = domain.MonitoringTotalProcessorTime;

				return BitConverter.GetBytes(survivedMemory + totalMemory + totalProcessorTime.Ticks);

				//TODO: problem here as first few bytes seem to be zero
			};
		}
	}
}
