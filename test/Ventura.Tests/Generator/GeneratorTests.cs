using System;
using System.Linq;
using System.Text;

using Ventura.Exceptions;
using Ventura.Generator;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using FluentAssertions;

namespace Ventura.Tests.Generator
{
    [TestClass]
    public class GeneratorTests
    {
        private VenturaGenerator concreteGenerator;

        [TestInitialize]
        public void Setup()
        {
            concreteGenerator = new VenturaGenerator();
        }

        [TestMethod]
        [ExpectedException(typeof(GeneratorInputException))]
        public void Generator_ThrowsException_When_InputArray_Zero()
        {
            var testArray = new byte[] { };
            concreteGenerator.GenerateData(testArray);
        }

        [TestMethod]
        [ExpectedException(typeof(GeneratorInputException))]
        public void Generator_ThrowsException_When_InputArray_GreaterThan_MaximumSize()
        {
            var testArray = new byte[15500000];
            var testGenerator = new TestGenerator();
            testGenerator.GenerateDatePerStateKey(testArray);
        }

        [TestMethod]
        public void Counter_IsCorrectly_Transformed_UponInitialization()
        {
            var testGenerator = new TestGenerator();
            var blockArray = testGenerator.TransformCounterToByteArray();
            var counter = BitConverter.ToInt32(blockArray, 0);

            counter.Should().Be(1);
        }

        [TestMethod]
        public void Generator_IsSeeded_UponInitialisation()
        {
            var testGenerator = new TestGenerator();
            testGenerator.IsSeeded().Should().Be(true);
        }

        [TestMethod]
        public void Counter_IsIncremented_AfterReseed()
        {
            var testGenerator = new TestGenerator();
            testGenerator.Reseed(new byte[] { });

            var blockArray = testGenerator.TransformCounterToByteArray();
            var counter = BitConverter.ToInt32(blockArray, 0);

            counter.Should().Be(2);
        }

        [TestMethod]
        public void Generator_Changes_StateKey_After_Request()
        {
            var testArray = new byte[10];
            var testGenerator = new TestGenerator();
            var initialKey = testGenerator.ReturnStateKey();
            testGenerator.GenerateDatePerStateKey(testArray);
            var updatedKey = testGenerator.ReturnStateKey();

            initialKey.Should().NotBeEquivalentTo(updatedKey);
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
            var generator = new VenturaGenerator(Cipher.Aes, seed);

            var input = new byte[1024];

            var firstOutput = generator.GenerateData(input);

            var otherGenerator = new VenturaGenerator(Cipher.Aes, seed);
            var secondOutput = otherGenerator.GenerateData(input);

            Assert.IsTrue(firstOutput.SequenceEqual(secondOutput));
        }

        [TestMethod]
        public void UniformRandomDistributionTest()
        {
            
        }
    }

    internal class TestGenerator : VenturaGenerator
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

        public bool IsSeeded()
        {
            return state.Seeded;
        }
    }
}
