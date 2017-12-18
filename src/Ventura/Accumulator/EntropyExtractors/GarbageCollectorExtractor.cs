using System;
using System.Collections.Generic;
using System.Text;
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
            var bytes = new byte[1] {0};
            return Task.Run(() => bytes); // stub, will fill in later
        }

        public IEnumerable<IEventEmitter> Events { get; }
    }
}
