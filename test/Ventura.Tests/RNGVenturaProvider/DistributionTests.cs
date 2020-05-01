using System.IO;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Testing;
using FluentAssertions;
using NUnit.Framework;

namespace Ventura.Tests.Prng
{
	[TestFixture]
	public class DistributionTests
	{

		[Test, Description("Runs a Kolgomorov-Smirnov test against the RNG's output compared to a " +
						   "uniform continuous distribution and a normal distribution")]
		public void RNGVentura_KolmogorovSmirnovTest_Approaches_Uniform_Continuous_Distribution()
		{
			double[] sample = GenerateFloatingPointNumberArray();

			var distribution = UniformContinuousDistribution.Standard;
			var uniformTest = new KolmogorovSmirnovTest(sample, distribution);

			var nDistribution = NormalDistribution.Standard;
			var normalTest = new KolmogorovSmirnovTest(sample, nDistribution);

			// no significant deviation from a uniform continuous distribution
			uniformTest.Significant.Should().BeFalse();
			// significant deviation from a normal distribution
			normalTest.Significant.Should().BeTrue();
		}

		[Test, Description("Passes the RNG's output through a Shapiro-Wilk test to assert" +
						   "if it deviates significantly from a normal distribution")]
		public void RNGVentura_ShapiroWilkTest_Has_Significant_Deviation_From_Normal_Distribution()
		{
			double[] sample = GenerateFloatingPointNumberArray();

			var swTest = new ShapiroWilkTest(sample);

			// significant deviation from a normal distribution
			swTest.Significant.Should().BeTrue();
		}

		[Test, Description("Passes the RNG's output through a Lilliefors test to assert" +
						   "if it deviates significantly from a normal distribution")]
		public void RNGVentura_LillieforsTest_Has_Significant_Deviation_From_Normal_Distribution()
		{
			double[] sample = GenerateFloatingPointNumberArray();

			var distribution = UniformContinuousDistribution.Estimate(sample);
			var ndistribution = NormalDistribution.Estimate(sample);

			var lillie = new LillieforsTest(sample, distribution);
			var nlillie = new LillieforsTest(sample, ndistribution);

			// no significant deviation from a uniform continuous distribution estimate
			lillie.Significant.Should().BeFalse();
			// significant deviation from a normal distribution
			nlillie.Significant.Should().BeTrue();
		}

		public double[] GenerateFloatingPointNumberArray()
		{
			double[] array;

			using var prng = RNGVenturaProviderFactory.Create(new MemoryStream());

			array = prng.NextDoubleArray(2, 1000);

			return array;
		}
	}
}
