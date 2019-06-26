using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Generator;
using Ventura.Interfaces;

namespace Ventura.Tests
{
	[TestClass]
	public class PrngVenturaTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void PrngVentura_Throws_ArgumentNullException_If_Accumulator_Null()
		{
			var ventura = new PrngVentura(null, new VenturaGenerator());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void PrngVentura_Throws_ArgumentNullException_If_Generator_Null()
		{
			var ventura = new PrngVentura(
				new VenturaAccumulator(new List<IEntropyExtractor> {new GarbageCollectorExtractor(0)}), null);
		}
	}
}
