using System;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors.Local
{
	/// <summary>
	/// Converts the system's coordinated universal time to a byte array
	/// </summary>
	internal class SystemUtcExtractor: EntropyExtractorBase, IEntropyExtractor
	{
		public SystemUtcExtractor(IEventEmitter eventEmitter) : base(eventEmitter)
		{
		}

		public override string SourceName { get; protected set; } = nameof(SystemUtcExtractor);

        public override Func<byte[]> GetEntropicData() => 
			() => BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
	}
}
