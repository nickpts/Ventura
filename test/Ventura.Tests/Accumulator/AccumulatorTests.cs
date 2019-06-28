using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Exceptions;
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

		//TODO: change to TestCase, with NUnit
		[Test, Description("Test that pool zero is used and cleared on even and odd reseeds")]
		public void Accumulator_Uses_Pool_Zero_On_Even_Reseed()
		{
			IsPoolUsed(1, 0).Should().BeTrue();
			IsPoolUsed(2, 0).Should().BeTrue();
			IsPoolUsed(5, 0).Should().BeTrue();
			IsPoolUsed(10, 0).Should().BeTrue();
		}

		[Test, Description("Test that first pool is used on every other ressed ")]
		public void Accumulator_Does_Not_Use_First_Pool_On_First_Reseed()
		{
			IsPoolUsed(1, 1).Should().BeFalse();
			IsPoolUsed(2, 1).Should().BeTrue();
			IsPoolUsed(3, 1).Should().BeFalse();
			IsPoolUsed(4, 1).Should().BeTrue();
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