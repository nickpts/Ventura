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

        public event Event.EntropyAvailabilityHander EntropyAvailable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNumber"></param>
        public GarbageCollectorExtractor(int sourceNumber)
        {
            this.sourceNumber = sourceNumber;
            //this.EntropyAvailable += new Event.EntropyAvailabilityHander();
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
            //IEvent massiveEvent = new Event(sourceNumber, ExtractEntropicData));
            
        }

        public Task<byte[]> ExtractEntropicData()
        {
            var bytes = new byte[1] {0};
            return Task.Run( () =>  bytes); // stub, will fill in later
        }
    }
}
