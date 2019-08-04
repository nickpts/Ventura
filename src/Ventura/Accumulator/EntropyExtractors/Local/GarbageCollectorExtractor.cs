using System;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors.Local
{
	/// <summary>
	/// Concatenates and returns information from the .NET garbage collector
	/// </summary>
	internal class GarbageCollectorExtractor : EntropyExtractorBase, IEntropyExtractor
    {
        public GarbageCollectorExtractor(IEventEmitter eventEmitter) : base(eventEmitter)
        {
        }

        public override string SourceName { get; protected set; } = nameof(GarbageCollectorExtractor);

        public override Func<byte[]> GetEntropicData()
        {
	        return () =>
	        {
		        var totalMemory = GC.GetTotalMemory(false);

		        var firstGen = GC.CollectionCount(0);
		        var secondGen = GC.CollectionCount(1);
		        var thirdGen = GC.CollectionCount(2);

		        return BitConverter.GetBytes(totalMemory + firstGen + secondGen + thirdGen);
	        };
        }
    }
}
