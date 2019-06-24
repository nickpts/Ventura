using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ventura.Accumulator;

namespace Ventura.Tests.Accumulator
{
    [TestClass]
    public class EntropyExtractorBaseTests
    {
        [TestInitialize]
        public void Setup()
        {
        }
		
        [TestMethod]
        public void EntropyExtractor_SuccessfulExtraction_ContainsEvent()
        {
            Func<byte[]> success = () => new byte[30];

            var extractor = new TestEntropyExtractor(1, success);
			extractor.EntropyAvailable += Extractor_EntropyAvailable;
            extractor.Run();
			
			// TODO: check that event was raised...
        }

		private void Extractor_EntropyAvailable(Event successfulExtraction)
		{
			
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
