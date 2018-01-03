using System;
using System.Collections.Generic;
using System.Linq;
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
            IEntropyExtractor firstExtractor = new GarbageCollectorExtractor(1);
            //IEntropyExtractor secondExtractor = new GarbageCollectorExtractor(1);

            accumulator = new VenturaAccumulator(new List<IEntropyExtractor> { firstExtractor });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Accumulator_ThrowsException_If_Passed_More_Than_MaxAmount_Of_Sources()
        {
            var extractors = new IEntropyExtractor[256];
            var extractorList = extractors.ToList();

            var accumulator = new VenturaAccumulator(extractorList);
        }
    }
}
