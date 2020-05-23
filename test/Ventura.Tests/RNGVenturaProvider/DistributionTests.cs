using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Accord.Diagnostics;
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

		[Test]
		public void RNG_Sequence_Ending_Bytes_Test()
		{
			byte[] array1 = new byte[12];
			byte[] array2 = new byte[12];

			using var prng1 = RNGVenturaProviderFactory.CreateSeeded(Cipher.Aes, ReseedEntropySourceGroup.Full);
			prng1.GetBytes(array1);

			foreach (var b in array1)
				System.Diagnostics.Debug.Write(b + " ");

			System.Diagnostics.Debug.WriteLine("");

			using var prng2 = RNGVenturaProviderFactory.CreateSeeded(Cipher.Aes, ReseedEntropySourceGroup.Full);
			prng2.GetBytes(array2);

			foreach (var b in array2)
				System.Diagnostics.Debug.Write(b + " ");
			
			System.Diagnostics.Debug.WriteLine("");

			Assert.AreNotEqual(array1[new Range(7, 12)], array2[new Range(7, 12)]);
		}

		[Test]
		public void RNG_Sequence_Ending_Numbers_Test()
		{
			var array1 = new int[10];
			var array2 = new int[10];

			using (var prng1 = RNGVenturaProviderFactory.CreateSeeded(Cipher.Aes, ReseedEntropySourceGroup.Local))
			{
				for (int i = 0; i < 10; i++)
				{
					int num = prng1.Next(1, 10);
					array1[i] = num;
				}
			}

			using (var prng2 = RNGVenturaProviderFactory.CreateSeeded(Cipher.Aes, ReseedEntropySourceGroup.Local))
			{
				for (int i = 0; i < 10; i++)
				{
					int num = prng2.Next(1, 10);
					array2[i] = num;
				}
			}

			Assert.AreNotEqual(array1[new Range(5, 10)], array2[new Range(5, 10)]);
		}

		private double[] GenerateFloatingPointNumberArray()
		{
			double[] array;

			using var prng = RNGVenturaProviderFactory.Create(new MemoryStream());

			array = prng.NextDoubleArray(2, 1000);

			return array;
		}
	}
}
