using System;
using System.Linq;
using System.Text;
using Ventura;
using Ventura.Exceptions;
using Ventura.Generator;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Org.BouncyCastle.Asn1.Crmf;

namespace Ventura.Tests.Generator
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
        public void Generator_ThrowsException_When_InputArray_Zero()
        {
            var testArray = new byte[] { };
            var result = aesGenerator.GenerateData(testArray);
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
        [ExpectedException(typeof(NotImplementedException))]
        public void Generator_ThrowsNotImplmented_On_GenerateBlocks()
        {
            var testArray = new byte[10];
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
        //    var result = aesGenerator.GenerateData(new byte[1]);

        //}

        //[TestMethod]
        //public void Generator_Changes_StateKey_After_Request()
        //{
        //    var testArray = new byte[10];
        //    var testGenerator = new TestGenerator();
        //    var initialKey = testGenerator.ReturnStateKey();
        //    testGenerator.GenerateDatePerStateKey(testArray);

        //    var updatedKey = testGenerator.ReturnStateKey();

        //    Assert.AreNotEqual(initialKey, updatedKey);
        //}

        [TestMethod]
        public void Generator_Returns_EncryptedData()
        {
            var testString = "All your base are belong to us";
            var inputBytes = Encoding.ASCII.GetBytes(testString);

            var result = aesGenerator.GenerateData(inputBytes);
            var outputString = Encoding.ASCII.GetString(result);

            Assert.IsFalse(inputBytes.Equals(outputString));
        }

        [TestMethod]
        public void Generator_InputOutputArrays_AreNotSequential()
        {
            var testString = "All your base are belong to us";
            var inputBytes = Encoding.ASCII.GetBytes(testString);

            var result = aesGenerator.GenerateData(inputBytes);

            Assert.IsFalse(inputBytes.SequenceEqual(result));
        }

        [TestMethod]
        public void Generator_WithSameSeed_ReturnsSameData()
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
