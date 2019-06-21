using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using FluentAssertions;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Exceptions;
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
            IEntropyExtractor secondExtractor = new AppDomainExtractor(2);

            accumulator = new VenturaAccumulator(new List<IEntropyExtractor> { firstExtractor, secondExtractor });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Accumulator_ThrowsException_If_Passed_More_Than_MaxAmount_Of_Sources()
        {
            var extractors = new IEntropyExtractor[256];
            var accumulator = new VenturaAccumulator(extractors.ToList());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
		public void Accumulator_ThrowsException_If_Not_Enough_Entropy_Collected()
        {
	        var extractors = new IEntropyExtractor[1];
	        var accumulator = new VenturaAccumulator(extractors.ToList());

	        accumulator.GetRandomDataFromPools(4);
        }

		[TestMethod]
		public void Accumulator_Throws_Exception()
		{
			accumulator.Distribute();
		}
	}
}