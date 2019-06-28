using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using FluentAssertions;

using NUnit.Framework;

using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Interfaces;

using static Ventura.Constants;

namespace Ventura.Tests.Accumulator
{
    [TestFixture]
    public class AccumulatorTests
    {
	    [Test]
	    public void Accumulator_ThrowsException_If_Passed_More_Than_MaxAmount_Of_Sources()
        {
            var extractors = new IEntropyExtractor[256];

            Assert.Throws(typeof(ArgumentException), () => new VenturaAccumulator(extractors.ToList()));
        }

        [Test]
		public void Accumulator_Initializes_Pools_On_Construction()
		{
			using (var accumulator = new TestAccumulator(new List<IEntropyExtractor>
				{new GarbageCollectorExtractor(1)}, default))
			{
				accumulator.EntropyPools.Count.Should().Be(MaximumNumberOfPools);
			}
		}

		[Test, Description("Test that pool zero is used and cleared on even and odd reseeds")]
		[TestCase(1, 0)]
		[TestCase(2, 0)]
		[TestCase(5, 0)]
		[TestCase(10, 0)]
		public void Accumulator_Uses_Pool_Zero_On_Even_Reseed(int reseedNumber, int poolNumber)
		{
			IsPoolUsed(reseedNumber, poolNumber).Should().BeTrue();
		}

		[Test, Description("Test that first pool is not used on odd reseeds (first, third) ")]
		[TestCase(1, 1)]
		[TestCase(3, 1)]
		public void Accumulator_Does_Not_Use_First_Pool_On_Odd_Reseeds(int reseedNumber, int poolNumber)
		{
			IsPoolUsed(reseedNumber, poolNumber).Should().BeFalse();
		}

		[TestCase(2, 1)]
		[TestCase(4, 1)]
		[Test, Description("Test that first pool is not used on even reseeds (second, fourth) ")]
		public void Accumulator_Uses_First_Pool_On_Even_Reseeds(int reseedNumber, int poolNumber)
		{
			IsPoolUsed(reseedNumber, poolNumber).Should().BeTrue();
		}

		public bool IsPoolUsed(int reseedNumber, int poolNumber)
		{
			var tokenSource = new CancellationTokenSource();

			using (var accumulator = new TestAccumulator(new List<IEntropyExtractor> { new GarbageCollectorExtractor(0) },
				tokenSource.Token))
			{
				while (!accumulator.HasEnoughEntropy)
				{
					Thread.Sleep(100);
				}

				tokenSource.Cancel(); // stop accumulation so that pool is not populated
				accumulator.GetRandomDataFromPools(reseedNumber); // start with reseed
				return accumulator.EntropyPools.ElementAt(poolNumber).ReadData().All(b => b == 0);
			}
		}
	}

    internal class TestAccumulator : VenturaAccumulator
    {
	    public TestAccumulator(IEnumerable<IEntropyExtractor> extractors, CancellationToken token)  :
		    base(extractors, token)
	    {

	    }

	    public List<EntropyPool> EntropyPools => Pools;
    }

}