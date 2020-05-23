using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Accord.Statistics.Testing;
using Ventura.Exceptions;
using Ventura.Generator;

using Moq;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Ventura.Interfaces;
using Range = System.Range;

namespace Ventura.Tests.Generator
{
	[TestFixture]
	public class GeneratorTests
	{
		private VenturaGenerator concreteGenerator;

		[SetUp]
		public void Setup()
		{
			concreteGenerator = new VenturaGenerator();
		}

		[Test]
		public void Generator_ThrowsException_When_InputArray_Zero()
		{
			var testArray = new byte[] { };

			Assert.Throws(typeof(GeneratorInputException), () => concreteGenerator.GenerateData(testArray));
		}

		[Test]
		public void Generator_ThrowsException_When_InputArray_GreaterThan_MaximumSize()
		{
			var testArray = new byte[15500000];

			void Test()
			{
				var testGenerator = new TestGenerator();
				testGenerator.GenerateDataPerStateKey(testArray);
			}

			Assert.Throws(typeof(GeneratorInputException), Test);
		}

		[Test]
		public void Counter_IsCorrectly_Transformed_UponInitialization()
		{
			var testGenerator = new TestGenerator();
			var blockArray = testGenerator.TransformCounterToByteArray();
			var counter = BitConverter.ToInt32(blockArray, 0);

			counter.Should().Be(1);
		}

		[Test]
		public void Generator_IsSeeded_UponInitialisation()
		{
			var testGenerator = new TestGenerator();
			testGenerator.IsSeeded().Should().Be(true);
		}

		[Test]
		public void Counter_IsIncremented_AfterReseed()
		{
			var testGenerator = new TestGenerator();
			testGenerator.Reseed(new byte[] { });

			var blockArray = testGenerator.TransformCounterToByteArray();
			var counter = BitConverter.ToInt32(blockArray, 0);

			counter.Should().Be(2);
		}

		[Test]
		public void Generator_Changes_StateKey_After_Request()
		{
			var testArray = new byte[10];
			var testGenerator = new TestGenerator();
			var initialKey = testGenerator.ReturnStateKey();

			testGenerator.GenerateDataPerStateKey(testArray);
			var updatedKey = testGenerator.ReturnStateKey();

			initialKey.Should().NotBeEquivalentTo(updatedKey);
		}

		[Test]
		public void Generator_Returns_EncryptedData()
		{
			var testString = "All your base are belong to us";
			var testBytes = Encoding.ASCII.GetBytes(testString);
			var inputBytes = Encoding.ASCII.GetBytes(testString);

			concreteGenerator.GenerateData(inputBytes);

			testBytes.Should().NotBeEquivalentTo(inputBytes);
		}

		[Test]
		public void Generator_InputOutputArrays_AreNotSequential()
		{
			var testString = "All your base are belong to us";
			var testBytes = Encoding.ASCII.GetBytes(testString);
			var inputBytes = Encoding.ASCII.GetBytes(testString);

			concreteGenerator.GenerateData(inputBytes);

			Assert.IsFalse(inputBytes.SequenceEqual(testBytes));
		}

		[Test]
		public void Generator_WithSameSeed_ReturnsSameData()
		{
			var seed = new byte[1];
			var generator = new VenturaGenerator(Cipher.Aes, seed);

			var input = new byte[1024];
			var secondInput = new byte[1024];

			generator.GenerateData(input);

			var otherGenerator = new VenturaGenerator(Cipher.Aes, seed);
			otherGenerator.GenerateData(secondInput);

			Assert.IsTrue(input.SequenceEqual(secondInput));
		}

		[Test] 
		public void Initialised_Generators_Return_DifferentData()
		{
			var input = new List<int>();
			var secondInput = new List<int>();

			var generator = new VenturaGenerator();

			for (int i = 0; i < 10; i++)
			{
				var array = new byte[4];
				generator.GenerateData(array);
				input.Add(Convert(array, 1, 10));
			}

			var otherGenerator = new VenturaGenerator();

			for (int i = 0; i < 10; i++)
			{
				var array2 = new byte[4];
				otherGenerator.GenerateData(array2);
				secondInput.Add(Convert(array2, 1, 10));
			}

			Assert.AreNotEqual(input.ToArray()[new Range(5, 10)], secondInput.ToArray()[new Range(5, 10)]);
		}

		private int Convert(byte[] array, int min, int max)
		{
			int num = Math.Abs(BitConverter.ToInt32(array, 0));

			return (num % (max - min)) + min;
		}

		internal class TestGenerator : VenturaGenerator
		{
			public new void GenerateDataPerStateKey(byte[] input) => base.GenerateDataPerStateKey(input);

			public byte[] TransformCounterToByteArray() => state.TransformCounterToByteArray();

			public byte[] ReturnStateKey() => state.Key;

			public bool IsSeeded() => state.Seeded;
		}
	}
}
