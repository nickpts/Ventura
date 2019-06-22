using System;
using System.Threading.Tasks;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors
{
    public class GarbageCollectorExtractor : EntropyExtractorBase, IEntropyExtractor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNumber"></param>
        public GarbageCollectorExtractor(int sourceNumber) : base(sourceNumber)
        {
        }

        public override string SourceName { get; protected set; } = ".NET CLR Garbage Collector";

        protected override Func<byte[]> ExtractEntropicData()
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
