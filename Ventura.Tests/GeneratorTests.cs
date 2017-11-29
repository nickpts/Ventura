using System;
using Ventura;
using Ventura.Exceptions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ventura.Tests
{
    [TestClass]
    public class GeneratorTests
    {
        private Generator testGenerator;

        [TestInitialize]
        public void Setup()
        {
            testGenerator = new Generator(Constants.Cipher.Aes);
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
    }
}
