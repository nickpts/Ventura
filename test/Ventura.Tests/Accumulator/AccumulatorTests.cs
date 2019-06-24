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
		//TODO: write tests
        private VenturaAccumulator accumulator;

        [TestInitialize]
        public void Setup()
        {

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
	        var accumulator = new VenturaAccumulator(new List<IEntropyExtractor> { new GarbageCollectorExtractor(1) });
	        accumulator.GetRandomDataFromPools(4);
        }

		[TestMethod]
		public void Accumulator_Throws_Exception()
		{
			//accumulator.Distribute();
		}
	}
}