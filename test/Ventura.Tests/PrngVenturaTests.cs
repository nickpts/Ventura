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

namespace Ventura.Tests
{
	[TestFixture]
	public class PrngVenturaTests
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
			Assert.Throws(typeof(ArgumentNullException), () => new PrngVentura(null, new VenturaGenerator(), stream.Object));
		}

		[Test]
		public void PrngVentura_Throws_ArgumentNullException_If_Generator_Null()
		{
			void Test()
			{
				var ventura = new PrngVentura(
					new VenturaAccumulator(new List<IEntropyExtractor> { new GarbageCollectorExtractor(0) }), null, stream.Object);
			}
			
			Assert.Throws(typeof(ArgumentNullException), Test);
		}

	}
}
