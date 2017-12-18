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
    public class Event: IEvent
    {
        private readonly int sourceNumber;
        private readonly Func<byte[]> extractorLogic;

        public delegate void EntropyAvailabilityHander(IEvent successfulExtraction);
        public event EntropyAvailabilityHander EntropyAvailable;

        public Event(int sourceNumber)
        {
            this.sourceNumber = sourceNumber;
        }

        public byte[] EntropicData { get; protected set; }

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

            EntropicData = result.ToArray();
        }
    }
}
