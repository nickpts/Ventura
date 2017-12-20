using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ventura.Tests.Accumulator
{
    [TestClass]
    public class EntropyExtractorTests
    {
        [TestInitialize]
        public void Setup()
        {

        }

        [TestMethod]
        public void SampleTest()
        {
            using (var extractor = new TestEntropyExtractor(1))
            {
                extractor.Start();
            }
        }
}

    public class TestEntropyExtractor : EntropyExtractorBase, IEntropyExtractor
    {
        public TestEntropyExtractor(int sourceNumber) : base(sourceNumber)
        {
        }

        protected override Task<byte[]> ExtractEntropicData()
        {
            var bytes = new byte[30];
            Func<byte[]> extraction = () => bytes; // return a maximum of 28 bytes;

            return Task.Run(extraction); // need to specify options here
        }
    }
}
