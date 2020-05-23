using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Interfaces;

using Moq;

namespace Ventura.Tests.Accumulator
{
    [TestFixture]
    public class EntropyExtractorBaseTests
    {
        private Mock<IEventEmitter> mockEmitter;

        [SetUp]
        public void Setup()
        {
            mockEmitter = new Mock<IEventEmitter>(0);
            mockEmitter.SetupAllProperties();
        }

        [Test]
        public void EntropyExtractor_SuccessfulExtraction_ContainsEvent()
        {
            Func<byte[]> test = () => new byte[30];
            var extractor = new TestEntropyExtractor(test, new EventEmitter(0));
            var events = new List<Event>();

            void ExtractorEntropyAvailable(Event successfulExtraction)
            {
                events.Add(successfulExtraction);
            }

            extractor.OnEntropyAvailable += ExtractorEntropyAvailable;
            extractor.Run();

            extractor.IsHealthy.Should().BeTrue();
            events.Should().NotBeEmpty();
        }

        [Test, Description("Tests that an extractor with count of failed events exceeding the threshold is not healthy ")]
        public void EntropyExtractor_With_Failed_Events_Is_Not_Healthy()
        {
            Func<byte[]> test = () => new byte[30];

            var failedEvent = new Event()
            {
                ExtractionSuccessful = false
            };

            Task<Event> extraction = Task.FromResult<Event>(failedEvent);
            mockEmitter.Setup(c => c.Execute(test)).Returns(extraction);

            var extractor = new TestEntropyExtractor(test, mockEmitter.Object);

            for (int i = 0; i <= 10; i++)
            {
                extractor.Run();
            }

            extractor.IsHealthy.Should().BeFalse();
        }
    }

    internal class TestEntropyExtractor : EntropyExtractorBase, IEntropyExtractor
    {
        private Func<byte[]> extractionLogic;

        public TestEntropyExtractor(Func<byte[]> extractionLogic, IEventEmitter eventEmitter) : base(eventEmitter) =>
            this.extractionLogic = extractionLogic;

        public override Func<byte[]> GetEntropicData() => extractionLogic;
    }
}
