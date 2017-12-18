using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            var massiveEvent = new Event(sourceNumber);
            massiveEvent.Execute(ExtractEntropicData());
            massiveEvent.EntropyAvailable += (x) => { events.Add(x); };
        }

        public Task<byte[]> ExtractEntropicData()
        {
            var bytes = new byte[1] {0};
            return Task.Run(() => bytes); // stub, will fill in later
        }
    }
}
