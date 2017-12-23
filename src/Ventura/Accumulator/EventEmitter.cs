using System;
using System.Linq;
using System.Threading.Tasks;

using Ventura.Interfaces;

using static Ventura.Constants;

namespace Ventura.Accumulator
{
    internal class EventEmitter : IEventEmitter
    {
        private readonly int sourceNumber;

        public delegate void EntropyAvailabilityHander(Event successfulExtraction);
        public event EntropyAvailabilityHander OnEntropyAvailable;

        public delegate void EventFailureHandler(Event failedExtraction);
        public event EventFailureHandler OnFailedEvent;

        public EventEmitter(int sourceNumber) => this.sourceNumber = sourceNumber;   
        
        public void Execute(Task<byte[]> extractionLogic)
        {
            try
            {
                byte[] data = extractionLogic.Result;

                var result = new byte[MaximumEventSize];
                byte sourceNumberByte = BitConverter.GetBytes(sourceNumber).First();
                byte dataLength = BitConverter.GetBytes(data.Length).First();

                result[0] = sourceNumberByte;
                result[1] = dataLength;

                Array.Copy(data, 0, result, 2, data.Length);
                Array.Clear(data, 0, data.Length);

                var @event = new Event {Data = result, ExtractionSuccessful = true};
                OnEntropyAvailable?.Invoke(@event);
            }
            catch (AggregateException aex)
            {
                //flatten, handle appropriately
                aex.Flatten();
                var @event = new Event { Exception = aex };
                OnFailedEvent?.Invoke(@event);
            }
        }
    }

    public class Event
    {
        public byte[] Data { get; internal set; }
        public bool ExtractionSuccessful { get; internal set; }
        public AggregateException Exception { get; internal set; }
    }
}