using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ventura.Accumulator.EntropyExtractors;

namespace Ventura.Tests.Accumulator
{
    [TestClass]
    public class EntropyExtractorTests
    {
        [TestMethod]
        public void GarbageCollectorExtractor_Returns_Data()
        {
            var extractor = new GarbageCollectorExtractor(0);
            extractor.Start();

            extractor.Events.Should().HaveCount(1);
        }

        [TestMethod]
        public void RemoteQuantumRngExtractor_Returns_Data()
        {
            var extractor = new RemoteQuantumRngExtractor(0);
            extractor.Start();

            extractor.Events.Should().HaveCount(1);
        }
    }
}
