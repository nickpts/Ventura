using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors
{
    public abstract class EntropyExtractorBase
    {
        private readonly EventEmitter eventEmitter;
        private readonly List<Event> events = new List<Event>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNumber"></param>
        protected EntropyExtractorBase(int sourceNumber)
        {
            eventEmitter = new EventEmitter(sourceNumber);
            eventEmitter.OnEntropyAvailable += OnEntropyAvailable_Append;
        }

        public virtual string SourceName { get; protected set; }

        public virtual IEnumerable<Event> Events
        {
            get
            {
                if (events.Count == 0)
                    throw new ArgumentException("No events produced yet!");

                return events;
            }
        }

        public void Start()
        {
            eventEmitter.Execute(ExtractEntropicData());
        }

        public virtual Task<byte[]> ExtractEntropicData()
        {
            throw new NotImplementedException("");
        }

        private void OnEntropyAvailable_Append(Event successfulExtraction)
        {
            events.Add(successfulExtraction);
        }
    }
}
