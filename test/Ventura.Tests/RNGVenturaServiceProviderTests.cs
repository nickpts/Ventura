using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Generator;
using Ventura.Interfaces;
using Moq;
using NUnit.Framework.Constraints;

namespace Ventura.Tests
{
	[TestFixture]
	public class RNGVenturaServiceProviderTests
	{
		private Mock<Stream> stream;

		[SetUp]
		public void Setup()
		{
			stream = new Mock<Stream>(MockBehavior.Default);
		}

		[Test]
		public void PrngVentura_Throws_ArgumentNullException_If_Accumulator_Null()
		{
			Assert.Throws(typeof(ArgumentNullException), () => new RNGVenturaServiceProvider(null, new VenturaGenerator(), stream.Object));
		}

		[Test]
		public void PrngVentura_Throws_ArgumentNullException_If_Generator_Null()
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
		public void PrngVentura_Throws_ArgumentException_If_Stream_Null()
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
		public void PrngVentura_Throws_Argument_Exception_If_Stream_Not_Writable()
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

	}
}
