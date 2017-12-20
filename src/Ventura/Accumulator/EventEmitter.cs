using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ventura.Exceptions;
using Ventura.Interfaces;

using static Ventura.Constants;

namespace Ventura.Accumulator
{
    public class EventEmitter : IEventEmitter
    {
        private readonly int sourceNumber;

        public delegate void EntropyAvailabilityHander(Event successfulExtraction);
        public event EntropyAvailabilityHander OnEntropyAvailable;

        public delegate void EventFailureHandler(Event failedExtraction);
        public event EventFailureHandler OnFailedExtraction;

        public EventEmitter(int sourceNumber) => this.sourceNumber = sourceNumber;   
        
        public void Execute(Task<byte[]> extractionLogic)
        {
            try
            {
                byte[] data = extractionLogic.Result;

                var result = new byte[MaximumEventSize];
                byte sourceNumberByte = BitConverter.GetBytes(sourceNumber).First();
                byte dataLength = BitConverter.GetBytes(data.Length).First();

                result.Append(sourceNumberByte);
                result.Append(dataLength);

                Array.Copy(data, 0, result, 4, data.Length);
                Array.Clear(data, 0, data.Length);

                var @event = new Event {Data = result, ExtractionSuccessful = true};
                OnEntropyAvailable?.Invoke(@event);
            }
            catch (AggregateException aex)
            {
                //flatten, handle appropriately
                var @event = new Event { };

                OnFailedExtraction?.Invoke(@event);
                Console.WriteLine("test");
            }
        }
    }

    public class Event
    {
        public byte[] Data { get; internal set; }
        public bool ExtractionSuccessful { get; internal set; }
        public EntropyEventFailedException Exception { get; internal set; }
    }
}