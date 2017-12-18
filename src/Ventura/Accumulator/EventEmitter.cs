using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Paddings;
using Ventura.Exceptions;
using Ventura.Interfaces;

namespace Ventura.Accumulator
{
    public class EventEmitter : IEventEmitter
    {
        private readonly int sourceNumber;
        private readonly Func<byte[]> extractorLogic;

        public delegate void EntropyAvailabilityHander(Event successfulExtraction);
        public event EntropyAvailabilityHander OnEntropyAvailable;

        public EventEmitter(int sourceNumber)
        {
            this.sourceNumber = sourceNumber;   
        }

        public void Execute(Task<byte[]> extractionLogic)
        {
            byte[] data = null;

            try
            {
                data = extractionLogic.Result;
            }
            catch (Exception ex)
            {
                throw new EntropyEventFailedException("message", ex.InnerException);
            }

            var result = new List<byte>(); // this will almost always be of a fixed size...
            byte sourceNumberByte = BitConverter.GetBytes(this.sourceNumber).First();
            byte dataLength = BitConverter.GetBytes(data.Length).First();

            result.Add(sourceNumberByte);
            result.Add(dataLength);
            result.AddRange(data);

            var @event = new Event { Data = result.ToArray() };

            OnEntropyAvailable?.Invoke(@event);
        }
    }

    public class Event
    {
        public byte[] Data { get; internal set; }
    }
}

