using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors
{
    public abstract class EntropyExtractorBase: IDisposable
    {
        private readonly EventEmitter eventEmitter;
        private readonly List<Event> events = new List<Event>();
        private readonly List<Event> failedEvents = new List<Event>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNumber"></param>
        protected EntropyExtractorBase(int sourceNumber)
        {
            eventEmitter = new EventEmitter(sourceNumber);
            eventEmitter.OnEntropyAvailable += OnEntropyAvailable_Append;
            eventEmitter.OnFailedEvent += OnFailedEvent_Append;
        }

        public virtual string SourceName { get; protected set; }

        public virtual IEnumerable<Event> Events
        {
            get
            {
                if (events.Count == 0)
                    throw new Exception("No events produced yet!");

                return events;
            }
        }

        public virtual IEnumerable<Event> FailedEvents => failedEvents; 

        public virtual void Start() => eventEmitter.Execute(ExtractEntropicData());

        protected virtual Task<byte[]> ExtractEntropicData() => throw new NotImplementedException("");

        private void OnEntropyAvailable_Append(Event extraction) => events.Add(extraction);

        private void OnFailedEvent_Append(Event extraction) => failedEvents.Add(extraction);

        public void Dispose()
        {
            eventEmitter.OnEntropyAvailable -= OnEntropyAvailable_Append;
            eventEmitter.OnFailedEvent -= OnFailedEvent_Append;
        }
    }
}