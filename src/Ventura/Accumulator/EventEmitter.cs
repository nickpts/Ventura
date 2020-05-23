using System;
using System.Linq;
using System.Threading.Tasks;

using Ventura.Interfaces;

using static Ventura.Constants;

namespace Ventura.Accumulator
{
    /// <summary>
    /// Runs extraction logic and emits an event. In accordance to spec the first byte
    /// in the random data contains the source number, the second the number of additional bytes of data
    /// and the third the actual data. If an exception occurs it is encapsulated and added to the event.
    /// </summary>
    internal class EventEmitter : IEventEmitter
    {
        public EventEmitter(int sourceNumber) => SourceNumber = sourceNumber;

        public int SourceNumber { get; set; }

        public Task<Event> Execute(Func<byte[]> extractionLogic)
        {
            try
            {
                var data = extractionLogic.Invoke();

                var result = new byte[MaximumEventSize];
                var sourceNumberByte = BitConverter.GetBytes(SourceNumber).First();
                var dataLength = BitConverter.GetBytes(data.Length).First();

                result[0] = sourceNumberByte;
                result[1] = dataLength;

                Array.Copy(data, 0, result, 2, data.Length);
                Array.Clear(data, 0, data.Length);

                var @event = new Event { SourceNumber = SourceNumber, Data = result, ExtractionSuccessful = true };

                return Task.FromResult(@event);
            }
            catch (Exception ex)
            {
                var @event = new Event { Exception = ex };

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
        public Exception Exception { get; internal set; }
    }
}