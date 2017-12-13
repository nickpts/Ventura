using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities.Collections;
using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropySources
{
    public class GarbageCollectorExtractor : IEntropyExtractor
    {
        private int sourceNumber;
        public List<IEvent> events;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNumber"></param>
        public GarbageCollectorExtractor(int sourceNumber)
        {
            this.sourceNumber = sourceNumber;
            events = new List<IEvent>();
        }

        public string SourceName { get; } = ".NET CLR Garbage Collector";

        public byte[] ExtractEntropicData()
        {
            return new byte[28]; // stub, will fill in later
        }

        public void EmitEvent()
        {
            
        }
    }
}
