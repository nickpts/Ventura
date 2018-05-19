using System;
using System.Text;
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

        protected override Task<byte[]> ExtractEntropicData()
        {
            byte[] extraction()
            {
                var totalMemory = GC.GetTotalMemory(false);

                var firstGen = GC.CollectionCount(0);
                var secondGen = GC.CollectionCount(1);
                var thirdGen = GC.CollectionCount(2);

                return BitConverter.GetBytes(totalMemory + firstGen + secondGen + thirdGen);
            }

            return Task.Run((Func<byte[]>) extraction); 
        }
    }
}
