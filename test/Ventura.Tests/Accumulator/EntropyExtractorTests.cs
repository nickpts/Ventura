using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;

using Moq;

namespace Ventura.Tests.Accumulator
{
    [TestFixture]
    public class EntropyExtractorTests
    {
		private readonly List<Event> receivedEvents = new List<Event>();

        [Test]
        public void GarbageCollectorExtractor_Returns_Data() => 
	        Test(new GarbageCollectorExtractor(new EventEmitter(0)));

        [Test]
        public void ProcessExtractor_Returns_Data() => 
	        Test(new ProcessEntropyExtractor(new EventEmitter(0)));

        [Test, Explicit]
        public void RemoteQuantumRngExtractor_Returns_Data() => 
	        Test(new QuantumRngExtractor(new EventEmitter(0)));
        
        [Test]
        public void RemoteNistRngExtractor_Returns_Data() =>
	        Test(new NistCsrngExtractor(new EventEmitter(0)));

        [Test]
        public void RngCryptoServiceProviderExtractor_Returns_Data() =>
	        Test(new RNGCryptoServiceProviderExtractor(new EventEmitter(0)));

		[Test]
		public void RemoteRandomOrgExtract_Returns_Data() =>
			Test(new RandomOrgExtractor(new EventEmitter(0)));

        public void Test(EntropyExtractorBase extractor)
        {
	        extractor.OnEntropyAvailable += Extractor_EntropyAvailable;
	        extractor.Run();

	        receivedEvents.Should().NotBeEmpty();
        }

		private void Extractor_EntropyAvailable(Event successfulExtraction) => receivedEvents.Add(successfulExtraction);
    }
}
