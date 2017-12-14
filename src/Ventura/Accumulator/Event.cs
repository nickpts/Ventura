using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Ventura.Interfaces;

namespace Ventura.Accumulator
{
    public class Event: IEvent
    {
        private readonly int sourceNumber;

        public Event(int sourceNumber, byte[] data)
        {
            this.sourceNumber = sourceNumber;
            EntropicData = GetConcatenatedEntropicData(data);
        }

        public byte[] EntropicData { get; }

        private byte[] GetConcatenatedEntropicData(byte[] sourceData)
        {
            var result = new List<byte>(); // this will almost always be of a fixed size...
            byte sourceNumberByte = BitConverter.GetBytes(this.sourceNumber).First();
            byte dataLength = BitConverter.GetBytes(sourceData.Length).First();

            result.Add(sourceNumberByte);
            result.Add(dataLength);
            result.AddRange(sourceData);

            return result.ToArray();
        }
    }
}
