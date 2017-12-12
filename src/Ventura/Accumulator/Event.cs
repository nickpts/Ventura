using System;
using System.Collections.Generic;
using System.Text;
using Ventura.Interfaces;

namespace Ventura.Accumulator
{
    public class Event: IEvent
    {
        private int sourceNumber;

        public Event(int sourceNumber)
        {
            this.sourceNumber = sourceNumber;
        }

        private byte[] GetEntropicData()
        {
            return new byte[] { };
        }

        public byte[] GetConcatenatedEntropicData()
        {
            var result = new List<byte>();
            //result.Add();
            return null;
        }
    }
}
