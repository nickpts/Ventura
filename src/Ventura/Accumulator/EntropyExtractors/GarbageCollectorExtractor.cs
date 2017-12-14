using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities.Collections;
using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors
{
    public class GarbageCollectorExtractor : IEntropyExtractor
    {
        private int sourceNumber;
        private List<IEvent> events = new List<IEvent>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNumber"></param>
        public GarbageCollectorExtractor(int sourceNumber)
        {
            this.sourceNumber = sourceNumber;
        }

        public IEnumerable<IEvent> Events
        {
            get
            {
                if (events.Count == 0)
                    throw new ArgumentException("No events produced yet!");
                
                return events;
            }
        }

        public string SourceName { get; } = ".NET CLR Garbage Collector";

        public void Start()
        {
            events.Add(new Event(sourceNumber, ExtractEntropicData()));
        }

        public byte[] ExtractEntropicData()
        {
            return new byte[28]; // stub, will fill in later
        }
    }
}
