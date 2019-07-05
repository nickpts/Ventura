using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;

namespace Ventura.Tests.Accumulator
{
    [TestFixture]
    public class EntropyExtractorTests
    {
		private readonly List<Event> receivedEvents = new List<Event>();

        [Test]
        public void GarbageCollectorExtractor_Returns_Data() => 
	        Test(new GarbageCollectorExtractor(0));

        [Test]
        public void ProcessExtractor_Returns_Data() => 
	        Test(new ProcessEntropyExtractor(0));

        [Test, Explicit]
        public void RemoteQuantumRngExtractor_Returns_Data() => 
	        Test(new RemoteQuantumRngExtractor(0));
        
        [Test]
        public void RemoteNistRngExtractor_Returns_Data() =>
	        Test(new RemoteNistCsrngExtractor(0));

        [Test]
        public void RngCryptoServiceProviderExtractor_Returns_Data() =>
	        Test(new RNGCryptoServiceProviderExtractor(0));

        public void Test(EntropyExtractorBase extractor)
        {
	        extractor.EntropyAvailable += Extractor_EntropyAvailable;
	        extractor.Run();

	        receivedEvents.Should().NotBeEmpty();
        }

		private void Extractor_EntropyAvailable(Event successfulExtraction) => receivedEvents.Add(successfulExtraction);
    }
}
