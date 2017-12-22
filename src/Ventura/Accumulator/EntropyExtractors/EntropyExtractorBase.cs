using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors
{
    public abstract class EntropyExtractorBase: IDisposable
    {
        protected readonly IEventEmitter eventEmitter;
        private readonly List<IEvent> events = new List<IEvent>();
        private readonly List<IEvent> failedEvents = new List<IEvent>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNumber"></param>
        protected EntropyExtractorBase(IEventEmitter eventEmitter)
        {
            this.eventEmitter = eventEmitter;
            eventEmitter.OnEntropyAvailable += OnEntropyAvailable_Append;
            eventEmitter.OnFailedEvent += OnFailedEvent_Append;
        }

        public virtual string SourceName { get; protected set; }

        internal virtual IEnumerable<IEvent> Events
        {
            get
            {
                if (events.Count == 0)
                    throw new Exception("No events produced yet!");

                return events;
            }
        }

        internal virtual IEnumerable<IEvent> FailedEvents => failedEvents; 

        public virtual void Start() => eventEmitter.Execute(ExtractEntropicData());

        protected virtual Task<byte[]> ExtractEntropicData() => throw new NotImplementedException("");

        private void OnEntropyAvailable_Append(IEvent extraction) => events.Add(extraction as Event);

        private void OnFailedEvent_Append(IEvent extraction) => failedEvents.Add(extraction as Event);

        public void Dispose()
        {
            eventEmitter.OnEntropyAvailable -= OnEntropyAvailable_Append;
            eventEmitter.OnFailedEvent -= OnFailedEvent_Append;
        }
    }
}