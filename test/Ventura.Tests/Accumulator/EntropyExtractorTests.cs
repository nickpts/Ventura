using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;

using Moq;
using Ventura.Accumulator.EntropyExtractors.Local;
using Ventura.Accumulator.EntropyExtractors.Remote;

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

        [Test]
        public void SystemUtcExtractor_Returns_Data() => 
	        Test(new SystemUtcExtractor(new EventEmitter(0)));

        [Test]
		public void RemoteRandomOrgExtract_Returns_Data() =>
			Test(new AtmosphericNoiceExtractor(new EventEmitter(0)));

		[Test]
		public void RemoteHotBitsExtractor_Returns_Data() =>
			Test(new HotBitsExtractor(new EventEmitter(0)));

		[Test]
		public void RemoteWeatherEntropyExtractor_Returns_Data() =>
			Test(new WeatherEntropyExtractor(new EventEmitter(0)));

		public void Test(EntropyExtractorBase extractor)
        {
	        extractor.OnEntropyAvailable += Extractor_EntropyAvailable;
	        extractor.Run();

	        receivedEvents.Should().NotBeEmpty();
        }

		private void Extractor_EntropyAvailable(Event successfulExtraction) => receivedEvents.Add(successfulExtraction);
    }
}
