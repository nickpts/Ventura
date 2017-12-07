using System;
using Ventura;
using Ventura.Exceptions;
using Ventura.Generator;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Ventura.Tests
{
    [TestClass]
    public class GeneratorTests
    {
        private VenturaPrng testGenerator;

        [TestInitialize]
        public void Setup()
        {
            testGenerator = new VenturaPrng(Constants.Cipher.Aes);
        }

        [TestMethod]
        [ExpectedException(typeof(GeneratorInputException))]
        public void GeneratorThrowsExceptionWhenInputArrayZero()
        {
            var testArray = new byte[] { };
            var result = testGenerator.GenerateData(testArray);
        }

        [TestMethod]
        [ExpectedException(typeof(GeneratorInputException))]
        public void GeneratorThrowsExceptionWhenInputArrayGreaterThanMaximumSize()
        {
            var testArray = new byte[1550000];
            var result = testGenerator.GenerateData(testArray);
        }

        [TestMethod]
        public void CounterIsCorrectlyTransformedIntoByteArray()
        {
            // need to make assembly visible to etc. 
        }

        [TestMethod]
        public void InitializeGeneratorCallsReseed()
        {
            var result = testGenerator.GenerateData(new byte[1]);

        }
    }
}
