using System;
using System.Linq;
using System.Text;

using Ventura.Exceptions;
using Ventura.Generator;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Org.BouncyCastle.Asn1.Crmf;
using FluentAssertions;
using Ventura.Interfaces;

namespace Ventura.Tests.Generator
{
    [TestClass]
    public class GeneratorTests
    {
        private VenturaPrng concreteGenerator;
        private Mock<IGenerator> mockGenerator;

        [TestInitialize]
        public void Setup()
        {
            concreteGenerator = new VenturaPrng();
            mockGenerator = new Mock<IGenerator>();
        }

        [TestMethod]
        [ExpectedException(typeof(GeneratorInputException))]
        public void Generator_ThrowsException_When_InputArray_Zero()
        {
            var testArray = new byte[] { };
            var result = concreteGenerator.GenerateData(testArray);
        }

        [TestMethod]
        [ExpectedException(typeof(GeneratorInputException))]
        public void Generator_ThrowsException_When_InputArray_GreaterThan_MaximumSize()
        {
            var testArray = new byte[1550000];
            var testGenerator = new TestGenerator();
            var result = testGenerator.GenerateDatePerStateKey(testArray);
        }

        [TestMethod]
        public void Counter_IsCorrectly_Transformed_UponInitialization()
        {
            var testGenerator = new TestGenerator();
            var blockArray = testGenerator.TransformCounterToByteArray();

            var counter = BitConverter.ToInt32(blockArray, 0);
            Assert.AreEqual(counter, 1);
        }

        [TestMethod]
        public void Counter_IsIncremented_AfterReseed()
        {
            var testGenerator = new TestGenerator();
            testGenerator.Reseed(new byte[] { });

            var blockArray = testGenerator.TransformCounterToByteArray();

            var counter = BitConverter.ToInt32(blockArray, 0);
            Assert.AreEqual(counter, 2);
        }

        //[TestMethod]
        //public void Initialize_GeneratorCalls_Reseed()
        //{
        //    mockGenerator.Setup(m => m.GenerateData(It.IsAny<byte[]>()));
        //    mockGenerator.Verify(m => m.Reseed(It.IsAny<byte[]>()));
        //}

        [TestMethod]
        public void Generator_Changes_StateKey_After_Request()
        {
            //var testArray = new byte[10];
            //var testGenerator = new TestGenerator();
            //var initialKey = testGenerator.ReturnStateKey();
            //testGenerator.GenerateDatePerStateKey(testArray);

            //var updatedKey = testGenerator.ReturnStateKey();

            //Assert.AreNotEqual(initialKey, updatedKey);

            mockGenerator.Setup(m => m.GenerateData(It.IsAny<byte[]>()));
            mockGenerator.Verify(m => m.UpdateKey());
        }

        [TestMethod]
        public void Generator_Returns_EncryptedData()
        {
            var testString = "All your base are belong to us";
            var inputBytes = Encoding.ASCII.GetBytes(testString);

            var result = concreteGenerator.GenerateData(inputBytes);
            var outputString = Encoding.ASCII.GetString(result);

            outputString.Should().NotBeSameAs(testString);
        }

        [TestMethod]
        public void Generator_InputOutputArrays_AreNotSequential()
        {
            var testString = "All your base are belong to us";
            var inputBytes = Encoding.ASCII.GetBytes(testString);

            var result = concreteGenerator.GenerateData(inputBytes);

            Assert.IsFalse(inputBytes.SequenceEqual(result));
        }

        [TestMethod]
        public void Generator_WithSameSeed_ReturnsSameData()
        {
            var seed = new byte[1];
            var generator = new VenturaPrng(Cipher.Aes, seed);

            var input = new byte[1024];

            var firstOutput = generator.GenerateData(input);

            var otherGenerator = new VenturaPrng(Cipher.Aes, seed);
            var secondOutput = otherGenerator.GenerateData(input);

            Assert.IsTrue(firstOutput.SequenceEqual(secondOutput));
        }

        [TestMethod]
        public void BcGenerator_ReturnsData()
        {
            var testString = "All your base are belong to us dear friend, noone knows but you";
            var inputBytes = Encoding.ASCII.GetBytes(testString);

            var gen = new VenturaPrng(Cipher.Serpent);
            var result = gen.GenerateData(inputBytes);

            Assert.IsFalse(inputBytes.SequenceEqual(result));
        }
    }

    public class TestGenerator : VenturaPrng
    {
        public byte[] GenerateDatePerStateKey(byte[] input)
        {
            return GenerateDataPerStateKey(input);
        }

        public byte[] TransformCounterToByteArray()
        {
            return state.TransformCounterToByteArray();
        }

        public byte[] ReturnStateKey()
        {
            return state.Key;
        }
    }
}
