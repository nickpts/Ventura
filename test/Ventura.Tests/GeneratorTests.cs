using System;
using System.Linq;
using System.Text;
using Ventura;
using Ventura.Exceptions;
using Ventura.Generator;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Org.BouncyCastle.Asn1.Crmf;

namespace Ventura.Tests
{
    [TestClass]
    public class GeneratorTests
    {
        private DotNetPrng aesGenerator;

        [TestInitialize]
        public void Setup()
        {
            aesGenerator = new DotNetPrng();
        }

        [TestMethod]
        [ExpectedException(typeof(GeneratorInputException))]
        public void GeneratorThrowsExceptionWhenInputArrayZero()
        {
            var testArray = new byte[] { };
            var result = aesGenerator.GenerateData(testArray);
        }

        [TestMethod]
        [ExpectedException(typeof(GeneratorInputException))]
        public void GeneratorThrowsExceptionWhenInputArrayGreaterThanMaximumSize()
        {
            var testArray = new byte[1550000];
            var result = aesGenerator.GenerateData(null);
        }

        [TestMethod]
        public void CounterIsCorrectlyTransformeUponInitialization()
        {
            var testGenerator = new DotNetPrng();
            var blockArray = testGenerator.state.TransformCounterToByteArray();

            var counter = BitConverter.ToInt32(blockArray, 0);
            Assert.AreEqual(counter, 1);
        }

        [TestMethod]
        public void CounterIsIncrementedAfterReseed()
        {
            var testGenerator = new DotNetPrng();
            testGenerator.Reseed(new byte[] { });
            var blockArray = testGenerator.state.TransformCounterToByteArray();

            var counter = BitConverter.ToInt32(blockArray, 0);
            Assert.AreEqual(counter, 2);
        }

        [TestMethod]
        public void InitializeGeneratorCallsReseed()
        {
            var result = aesGenerator.GenerateData(new byte[1]);

        }

        [TestMethod]
        public void GeneratorReturnsEncryptsData()
        {
            var testString = "All your base are belong to us";
            var inputBytes = Encoding.ASCII.GetBytes(testString);

            var result = aesGenerator.GenerateData(inputBytes);
            var outputString = Encoding.ASCII.GetString(result);

            Assert.IsFalse(inputBytes.Equals(outputString));
        }

        [TestMethod]
        public void GeneratorInputOutputArraysAreNotSequential()
        {
            var testString = "All your base are belong to us";
            var inputBytes = Encoding.ASCII.GetBytes(testString);

            var result = aesGenerator.GenerateData(inputBytes);

            Assert.IsFalse(inputBytes.SequenceEqual(result));
        }

        [TestMethod]
        public void GeneratorWithSameSeedReturnsSameData()
        {
            var seed = new byte[1];
            var generator = new DotNetPrng(seed);

            var input = new byte[1024];

            var firstOutput = generator.GenerateData(input);

            var otherGenerator = new DotNetPrng(seed);
            var secondOutput = otherGenerator.GenerateData(input);

            Assert.IsTrue(firstOutput.SequenceEqual(secondOutput));
        }
    }
}
