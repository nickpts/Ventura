using System;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors.Local
{
	/// <summary>
	/// Converts the system's coordinated universal time to a byte array
	/// </summary>
	public class SystemUtcExtractor: EntropyExtractorBase, IEntropyExtractor
	{
		public SystemUtcExtractor(IEventEmitter eventEmitter) : base(eventEmitter)
		{
		}

		public override string SourceName { get; protected set; } = nameof(SystemUtcExtractor);

		protected override Func<byte[]> ExtractEntropicData() => 
			() => BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
	}
}
