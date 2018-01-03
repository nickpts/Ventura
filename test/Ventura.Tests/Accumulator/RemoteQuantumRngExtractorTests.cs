using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ventura.Accumulator.EntropyExtractors;

namespace Ventura.Tests.Accumulator
{
    [TestClass]
    public class RemoteQuantumRngExtractorTests
    {
        [TestMethod]
        public void RemoteQuantiumRngExtractor_Returns_Data()
        {
            var extractor = new RemoteQuantumRngExtractor(0);
            extractor.Start();

            extractor.Events.Should().HaveCount(1);
        }
    }
}
