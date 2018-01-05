using System;
using System.Collections.Generic;
using System.Text;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Generator;
using Ventura.Interfaces;

namespace Ventura
{
    internal class PRNGVenturaService : IVenturaService
    {
        private readonly PRNGVenturaServiceSettings settings;
        private readonly VenturaAccumulator accumulator;
        private readonly VenturaGenerator generator;

        private int ReSeedCounter = 0;

        public PRNGVenturaService(PRNGVenturaServiceSettings settings,
            VenturaAccumulator accumulator,
            VenturaGenerator generator)
        {
            this.settings = settings;
        }

        public void InitialisePRNG()
        {
            accumulator.Collect();
            generator = new VenturaGenerator();
        }

        public byte[] GetRandomData()
        {
            throw new NotImplementedException();
        }
    }
}
