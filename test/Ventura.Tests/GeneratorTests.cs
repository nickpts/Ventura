using System;
using Ventura;
using Ventura.Exceptions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ventura.Generator;

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
            var result = testGenerator.GenerateRandomData(testArray);
        }

        [TestMethod]
        [ExpectedException(typeof(GeneratorInputException))]
        public void GeneratorThrowsExceptionWhenInputArrayGreaterThanMaximumSize()
        {
            var testArray = new byte[1550000];
            var result = testGenerator.GenerateRandomData(testArray);
        }

        [TestMethod]
        public void CounterIsCorrectlyTransformedIntoByteArray()
        {
            // need to make assembly visible to etc. 
        }
    }
}
