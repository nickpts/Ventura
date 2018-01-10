using System;
using System.Collections.Generic;
using System.Text;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Generator;
using Ventura.Interfaces;

namespace Ventura
{
    internal class PRNGVenturaService : IPRNGVenturaService
    {
        private readonly IAccumulator accumulator;
        private readonly IGenerator generator;

        private int reseedCounter = 0;

        public PRNGVenturaService(IAccumulator accumulator, IGenerator generator)
        {
            this.accumulator = accumulator;
            this.generator = generator;
        }

        public void InitialisePRNG()
        {
            
        }

        public byte[] GetRandomData()
        {
            return new byte[] {};
        }

        public int GetRandomNumber()
        {
            throw new NotImplementedException();
        }

        public int[] GetRandomNumbers()
        {
            throw new NotImplementedException();
        }

        public string GetRandomString(int length)
        {
            throw new NotImplementedException();
        }

        public string[] GetRandomStrings(int length)
        {
            throw new NotImplementedException();
        }

        public string[] GetRandomStrings(int minStringLength, int maxStringLength, int arrayLength)
        {
            throw new NotImplementedException();
        }
    }
}