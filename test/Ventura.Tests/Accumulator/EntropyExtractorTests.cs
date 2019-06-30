using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;

namespace Ventura.Tests.Accumulator
{
    [TestFixture()]
    public class EntropyExtractorTests
    {
		List<Event> receivedEvents = new List<Event>();

        [Test]
        public void GarbageCollectorExtractor_Returns_Data()
        {
            var extractor = new GarbageCollectorExtractor(0);
			extractor.EntropyAvailable += Extractor_EntropyAvailable;

			for (int i = 0; i < 1000; i++)
			{
				extractor.Run();
				GC.Collect(0);
				GC.Collect(1);
				GC.Collect(2);
			}

            receivedEvents.Should().NotBeEmpty();
        }

        [Test, Explicit]
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
