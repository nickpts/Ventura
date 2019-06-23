using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ventura.Interfaces;

using static Ventura.Constants;

namespace Ventura.Accumulator
{
    internal class EventEmitter : IEventEmitter
    {
        private readonly int sourceNumber;

        public EventEmitter(int sourceNumber) => this.sourceNumber = sourceNumber;
        
        public Task<Event> Execute(Func<byte[]> extractionLogic)
        {
            try
            {
	            var data = extractionLogic.Invoke();

	            var result = new byte[MaximumEventSize];
                var sourceNumberByte = BitConverter.GetBytes(sourceNumber).First();
                var dataLength = BitConverter.GetBytes(data.Length).First();

                result[0] = sourceNumberByte;
                result[1] = dataLength;

                Array.Copy(data, 0, result, 2, data.Length);
                Array.Clear(data, 0, data.Length);

                var @event = new Event { SourceNumber = sourceNumber, Data = result, ExtractionSuccessful = true };

                return Task.FromResult(@event);
            }
            catch (AggregateException aex)
            {
                //flatten, handle appropriately
                aex.Flatten();
                var @event = new Event { Exception = aex };

				//TODO: need to handle appropriately here
                return Task.FromResult(@event);
            }
        }
    }

    public class Event
    {
		public int SourceNumber { get; internal set; }
        public byte[] Data { get; internal set; }
        public bool ExtractionSuccessful { get; internal set; }
        public AggregateException Exception { get; internal set; }
    }
}