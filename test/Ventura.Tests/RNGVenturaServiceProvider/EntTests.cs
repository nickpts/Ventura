using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace Ventura.Tests.Prng
{
	/*
	 * Uses John Walker's ent program (https://www.fourmilab.ch/random/), to find the chi-square distribution
	 * of a 100mb test file containing a sequence of bytes. The chi-square test is the most commonly used
	 * test for the randomness of data and is extremely sensitive to errors in pseudo-number sequence 
	 * generators. The percentage is interpreted as the degree to which the sequence tested is suspect of being
	 * non-random. If the percentage is greater than 99% or less than 1% the sequence is almost certainly not random. 
	 * If the percentage is between 99% and 95% or between 1 and 5% the sequence is suspect. Percentages between 
	 * 90% and 95% and 5% and 10% indicate the sequence is "almost suspect". For the purpose of this test, if a
	 * the chi-square indicates an almost suspect percentage the test is considered failed.
	 */
	[TestFixture]
	public class EntTests
	{
		private static string TestFilePath = Directory.GetCurrentDirectory() + @"\ent\testfile";
		private static string WorkingDirectory = Directory.GetCurrentDirectory() + @"\ent\";
		private static string EntExePath = WorkingDirectory + @"ent.exe";
		private static string Arguments = "-b testfile";
		private static string ChiSquareDistribution;

		[Test, Explicit]
		public void Ent_Test_Suite_Chi_Square_Entropy_Is_Outside_Suspect_Ranges()
		{
			try
			{
				GenerateTestFile();
				RunEntTest();
				AssertChiSquare();
			}
			finally
			{
				DeleteTestFile();
			}
		}

		public void GenerateTestFile()
		{
			byte[] array = new byte[100_048_576];

			using (var prng = RNGVenturaServiceProviderFactory.Create(new MemoryStream(), Cipher.Aes, ReseedEntropySourceGroup.Full))
			{
				prng.GetBytes(array);
			}

			File.WriteAllBytes(TestFilePath, array);
		}

		public void RunEntTest()
		{
			var process = new Process();
			process.StartInfo = new ProcessStartInfo()
			{
				Arguments = Arguments,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				WorkingDirectory = WorkingDirectory,
				FileName = EntExePath
			};

			process.Start();

			bool found = false;
			int consoleLineCounter = 0;

			while (!process.StandardOutput.EndOfStream)
			{
				string line = process.StandardOutput.ReadLine();


				if (line.Contains("Chi square distribution"))
				{
					found = true;
				}

				if (consoleLineCounter == 6 && !string.IsNullOrEmpty(line))
				{
					ChiSquareDistribution = line;
					break;
				}

				consoleLineCounter++;
			}

			found.Should().BeTrue("If no chi square distribution in stdout, ent.exe not working properly");
			process.WaitForExit();
		}

		private void AssertChiSquare()
		{
			var match = Regex.Match(ChiSquareDistribution, @"\d+(\.\d+)?");
			var result = match.Value;

			double chiSquare = Convert.ToDouble(result);

			if (chiSquare >= 99d || chiSquare <= 1d)
			{
				Assert.Fail($"Sequence with chi-square distribution: {chiSquare} is not random");
			}

			if ((chiSquare < 99d && chiSquare >= 95d) || (chiSquare > 1d && chiSquare <= 5d))
			{
				Assert.Fail($"Sequence with chi-square distribution: {chiSquare} is suspect");
			}

			if ((chiSquare < 95d && chiSquare >= 90d) || (chiSquare > 5 && chiSquare <= 10d))
			{
				Assert.Warn($"Sequence with chi-square distribution: {chiSquare} is almost suspect");
			}

			Assert.Pass($"Sequence with chi-square distribution: {chiSquare} is within acceptable limits");
		}

		public void DeleteTestFile() => File.Delete(TestFilePath);
	}
}
