using System;
using System.Collections.Generic;

using Moq;
using NUnit.Framework;
using Ventura.Interfaces;

namespace Ventura.Tests.Prng
{
    public class RemoteSeedProviderTests
    {
        [Test]
        public void SeedProvider_Throws_ArgumentNullException_If_Extractors_Null() =>
            Assert.Throws<ArgumentNullException>(() => new SeedProvider(null));

        []
        public void SeedProvider_Throws_ArgumentException_If_Extractors_Emptyl() =>
            Assert.Throws<ArgumentNullException>(() => new SeedProvider(new List<IEntropyExtractor>()));

        [Test]
        public void GetBytes_Throws_InvalidOperation_Exception_If_All_Attempts_Unsuccessful()
        {
            var mockExtractorOne = new Mock<IEntropyExtractor>();
            var mockExtractorTwo = new Mock<IEntropyExtractor>();

            mockExtractorOne.Setup(x => x.GetEntropicData()).Throws(new Exception());
            mockExtractorTwo.Setup(x => x.GetEntropicData()).Throws(new Exception());

            var extractors = new List<IEntropyExtractor>
            {
                mockExtractorOne.Object,
                mockExtractorTwo.Object,
            };

            var provider = new SeedProvider(extractors);

            Assert.Throws<InvalidOperationException>(() => provider.GetBytes());
        }
    }
}
