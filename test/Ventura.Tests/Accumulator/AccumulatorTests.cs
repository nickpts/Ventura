using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using FluentAssertions;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Interfaces;

namespace Ventura.Tests.Accumulator
{
    [TestClass]
    public class AccumulatorTests
    {
        private VenturaAccumulator accumulator;

        [TestInitialize]
        public void Setup()
        {
            IEntropyExtractor firstExtractor = new GarbageCollectorExtractor(0);
            //IEntropyExtractor secondExtractor = new GarbageCollectorExtractor(1);

            accumulator = new VenturaAccumulator(new List<IEntropyExtractor> { firstExtractor });
        }

        [TestMethod]
        public void RunAccumulator()
        {
            accumulator.Collect();
        }
    }
}
