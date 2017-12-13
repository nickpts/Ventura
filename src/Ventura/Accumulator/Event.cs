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
        private int sourceNumber;
        private byte[] entropicData;

        public Event(int sourceNumber, byte[] entropicData)
        {
            this.sourceNumber = sourceNumber;
            this.entropicData = entropicData;
        }

        private byte[] GetEntropicData()
        {
            return new byte[] { };
        }

        public byte[] GetConcatenatedEntropicData()
        {
            var result = new List<byte>();
            byte sourceNumberByte = BitConverter.GetBytes(this.sourceNumber).First();
            byte dataLength = BitConverter.GetBytes(entropicData.Length).First();

            result.Add(sourceNumberByte);
            result.Add(dataLength);
            result.AddRange(entropicData);

            return result.ToArray();
        }
    }
}
