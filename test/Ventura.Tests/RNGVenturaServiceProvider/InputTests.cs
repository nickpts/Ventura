using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;

using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Generator;
using Ventura.Interfaces;

namespace Ventura.Tests
{
	[TestFixture]
	public class InputTests
	{
		private Mock<Stream> stream;

		[SetUp]
		public void Setup()
		{
			stream = new Mock<Stream>(MockBehavior.Default);
		}

		[Test]
		public void RNGVentura_Throws_ArgumentNullException_If_Accumulator_Null()
		{
			Assert.Throws(typeof(ArgumentNullException), () => new RNGVenturaServiceProvider(null, new VenturaGenerator(), stream.Object));
		}

		[Test]
		public void RNGVentura_Throws_ArgumentNullException_If_Generator_Null()
		{
			void Test()
			{
				var ventura = new RNGVenturaServiceProvider(
					new VenturaAccumulator(
						new List<IEntropyExtractor>
						{
							new GarbageCollectorExtractor(new EventEmitter(0))
						}), null, stream.Object);
			}

			Assert.Throws(typeof(ArgumentNullException), Test);
		}

		[Test]
		public void RNGVentura_Throws_ArgumentException_If_Stream_Null()
		{
			void Test()
			{
				var ventura = new RNGVenturaServiceProvider(
					new VenturaAccumulator(
						new List<IEntropyExtractor>
						{
							new GarbageCollectorExtractor(new EventEmitter(0))
						}), new VenturaGenerator(), null);
			}

			Assert.Throws(typeof(ArgumentException), Test);
		}

		[Test]
		public void RNGVentura_Throws_Argument_Exception_If_Stream_Not_Writable()
		{
			void Test()
			{
				var nonWritablestream = new MemoryStream(new byte[1], false);

				var ventura = new RNGVenturaServiceProvider(
					new VenturaAccumulator(
						new List<IEntropyExtractor>
						{
							new GarbageCollectorExtractor(new EventEmitter(0))
						}), new VenturaGenerator(), nonWritablestream);
			}

			Assert.Throws(typeof(ArgumentException), Test);
		}

		[Test]
		public void RNGVentura_GetRandomNumber_Throws_Exception_For_Negative_Values()
		{
			void Test()
			{
				using (var prng = RNGVenturaServiceProviderFactory.Create(new MemoryStream()))
				{
					var result = prng.GetRandomNumber(-1, 10);
				}
			}

			Assert.Throws<ArgumentException>(Test);
		}

		[Test]
		public void RNGVentura_GetRandomNumbers_Throws_Exception_For_Negative_Values()
		{
			void Test()
			{
				using (var prng = RNGVenturaServiceProviderFactory.Create(new MemoryStream()))
				{
					var result = prng.GetRandomNumbers(-1, 10, 1000);
				}
			}

			Assert.Throws<ArgumentException>(Test);
		}

		[Test]
		public void RNGVentura_GetRandomNumbers_Throws_Exception_For_Zero_Or_Negative_Array_Length_Values()
		{
			void ZeroTest()
			{
				using (var prng = RNGVenturaServiceProviderFactory.Create(new MemoryStream()))
				{
					var result = prng.GetRandomNumbers(1, 10, -1);
				}
			}

			void NegativeTest()
			{
				using (var prng = RNGVenturaServiceProviderFactory.Create(new MemoryStream()))
				{
					var result = prng.GetRandomNumbers(1, 10, 0);
				}
			}

			Assert.Throws<ArgumentException>(ZeroTest);
			Assert.Throws<ArgumentException>(NegativeTest);
		}

		[Test]
		public void RNGVentura_GetRandomNumber_Throws_Exception_If_Round_To_Decimals_Less_Than_Zero()
		{
			void NegativeTest()
			{
				using (var prng = RNGVenturaServiceProviderFactory.Create(new MemoryStream()))
				{
					var result = prng.GetRandomNumber(-1);
				}
			}

			Assert.Throws<ArgumentException>(NegativeTest);
		}

		[Test]
		public void RNGVentura_GetRandomNumbers_Throws_Exception_If_Round_To_Decimals_Less_Than_Zero()
		{
			void NegativeTest()
			{
				using (var prng = RNGVenturaServiceProviderFactory.Create(new MemoryStream()))
				{
					var result = prng.GetRandomNumbers(-1, 10);
				}
			}

			Assert.Throws<ArgumentException>(NegativeTest);
		}

		[Test]
		public void RNGVentura_GetRandomNumbers_Throws_Exception_If_Length_Zero_Or_Less_Than_Zero()
		{
			void ZeroTest()
			{
				using (var prng = RNGVenturaServiceProviderFactory.Create(new MemoryStream()))
				{
					var result = prng.GetRandomNumbers(2, 0);
				}
			}

			void NegativeTest()
			{
				using (var prng = RNGVenturaServiceProviderFactory.Create(new MemoryStream()))
				{
					var result = prng.GetRandomNumbers(2, -1);
				}
			}

			Assert.Throws<ArgumentException>(ZeroTest);
			Assert.Throws<ArgumentException>(NegativeTest);
		}
	}
}
