using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Generator;
using Ventura.Interfaces;

namespace Ventura.Tests
{
	[TestFixture]
	public class PrngVenturaTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void PrngVentura_Throws_ArgumentNullException_If_Accumulator_Null()
		{
			Assert.Throws(typeof(ArgumentNullException), () => new PrngVentura(null, new VenturaGenerator()));
		}

		[Test]
		public void PrngVentura_Throws_ArgumentNullException_If_Generator_Null()
		{
			void Test()
			{
				var ventura = new PrngVentura(
					new VenturaAccumulator(new List<IEntropyExtractor> { new GarbageCollectorExtractor(0) }), null);
			}
			
			Assert.Throws(typeof(ArgumentNullException), Test);
		}
	}
}
