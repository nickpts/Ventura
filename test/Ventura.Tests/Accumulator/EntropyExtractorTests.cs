using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;

namespace Ventura.Tests.Accumulator
{
    [TestClass]
    public class EntropyExtractorTests
    {
		List<Event> receivedEvents = new List<Event>();

        [TestMethod]
        public void GarbageCollectorExtractor_Returns_Data()
        {
            var extractor = new GarbageCollectorExtractor(0);
			extractor.EntropyAvailable += Extractor_EntropyAvailable;

            extractor.Run();
            receivedEvents.Should().NotBeEmpty();
        }

        [TestMethod]
        public void RemoteQuantumRngExtractor_Returns_Data()
        {
	        var extractor = new RemoteQuantumRngExtractor(0);
	        extractor.EntropyAvailable += Extractor_EntropyAvailable;

	        extractor.Run();
	        receivedEvents.Should().NotBeEmpty();
        }

		private void Extractor_EntropyAvailable(Event successfulExtraction) => receivedEvents.Add(successfulExtraction);
		
    }
}
