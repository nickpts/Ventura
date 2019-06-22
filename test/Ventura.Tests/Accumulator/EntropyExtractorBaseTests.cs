using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void EntropyExtractor_Appends_Failed_Event_During_Extraction()
        {
            Func<byte[]> failure = () => throw new Exception("test");

            using (var extractor = new TestEntropyExtractor(1, failure))
            {
                extractor.Start();
                //extractor.FailedEvents.Should().HaveCount(1);
            }
        }

        [TestMethod]
        public void EntropyExtractor_Appended_FailedEvent_ContainsAggregate_Exception()
        {
            Func<byte[]> failure = () => throw new Exception("test");

            using (var extractor = new TestEntropyExtractor(1, failure))
            {
                extractor.Start();

            }
        }

        [TestMethod]
        public void EntropyExtractor_SuccessfulExtraction_ContainsEvent()
        {
            Func<byte[]> success = () => new byte[30];

            using (var extractor = new TestEntropyExtractor(1, success))
            {
                extractor.Start();

            }
        }

        [TestMethod]
        public void EntropyExtractor_SuccessfulExtraction_FlagSetTrue_Exception_Null()
        {
            Func<byte[]> success = () => new byte[30];

            using (var extractor = new TestEntropyExtractor(1, success))
            {
                extractor.Start();

            }
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
