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
    }
}
