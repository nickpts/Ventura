using System;
using System.Threading.Tasks;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors
{
    public class GarbageCollectorExtractor: EntropyExtractorBase, IEntropyExtractor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNumber"></param>
        public GarbageCollectorExtractor(int sourceNumber) : base(sourceNumber)
        {
        }

        public override string SourceName { get; protected set; } = ".NET CLR Garbage Collector";

        public override Task<byte[]> ExtractEntropicData()
        {
            var bytes = new byte[1] { 0 };
            Func<byte[]> extraction = () => bytes; // return a maximum of 28 bytes;

            return Task.Run(extraction); // need to specify options here
        }
    }
}
