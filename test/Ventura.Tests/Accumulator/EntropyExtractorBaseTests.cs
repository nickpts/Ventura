using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Interfaces;

namespace Ventura.Tests.Accumulator
{
	[TestFixture]
    public class EntropyExtractorBaseTests
    {
	    [Test]
        public void EntropyExtractor_SuccessfulExtraction_ContainsEvent()
        {
            Func<byte[]> success = () => new byte[30];
            var extractor = new TestEntropyExtractor(1, success);
            var events = new List<Event>();

            void ExtractorEntropyAvailable(Event successfulExtraction)
            {
				events.Add(successfulExtraction);
            }

			extractor.EntropyAvailable += ExtractorEntropyAvailable;
            extractor.Run();

            events.Should().NotBeEmpty();
        }
    }

    public class TestEntropyExtractor : EntropyExtractorBase, IEntropyExtractor
    {
        private Func<byte[]> extractionLogic;

        public TestEntropyExtractor(int sourceNumber, Func<byte[]> extractionLogic) : base(sourceNumber) =>
            this.extractionLogic = extractionLogic;

        protected override Func<byte[]> ExtractEntropicData() => extractionLogic;
    }
}
