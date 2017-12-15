using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto.Paddings;
using Ventura.Interfaces;

namespace Ventura.Accumulator
{
    public class Event: IEvent
    {
        private readonly int sourceNumber;
        private readonly Func<byte[]> extractorLogic;

        public delegate byte[] EntropyAvailabilityHander();
        public event EntropyAvailabilityHander EntropyAvailable;

        public Event(int sourceNumber, Func<byte[]> extractorLogic)
        {
            this.sourceNumber = sourceNumber;
            this.extractorLogic = extractorLogic;
        }

        public byte[] Trigger()
        {
            byte[] data;

            try
            {
                data = extractorLogic.Invoke();
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

            return result.ToArray();
        }
    }

    internal class EntropyEventFailedException : Exception
    {
        public EntropyEventFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
