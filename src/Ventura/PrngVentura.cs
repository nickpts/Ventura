using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Ventura.Exceptions;
using Ventura.Interfaces;

using static Ventura.Constants;

namespace Ventura
{
	internal class PrngVentura : IPrngVentura
	{
		private readonly IAccumulator accumulator;
		private readonly IGenerator generator;
		private DateTimeOffset lastReseedTime = DateTimeOffset.MinValue;
		private readonly Stream stream;
		private readonly object syncRoot = new object();
		private static readonly TimeSpan seedUpdateInterval = TimeSpan.FromMinutes(10);
		private int reseedCounter;
		private Timer reseedTimer;

		public PrngVentura(IAccumulator accumulator, IGenerator generator, Stream stream)
		{
			this.accumulator = accumulator ?? throw new ArgumentNullException();
			this.generator = generator ?? throw new ArgumentNullException();
			this.stream = stream ?? throw new ArgumentException();

			if (!stream.CanRead || !stream.CanSeek || !stream.CanWrite)
					throw new ArgumentException("Stream must be readable/writable/seekable");
		}

		/// <summary>
		/// Reads the first 64 bytes from the seed stream and uses it to reseed the generator.
		/// Runs a task on regular intervals that generates a new seed and writes it to the stream.
		/// </summary>
		public void Initialise()
		{
			var seed = new byte[SeedFileSize];
			stream.Read(seed, 0, SeedFileSize);
			generator.Reseed(seed); 

			reseedTimer = new Timer(UpdateSeed, null, 0, seedUpdateInterval.Milliseconds);
		}

		/// <summary>
		/// Returns data from generator, reseeds every time pool 0 has enough entropy or
		/// a set amount of time (100ms according to spec) has passed between reseeds
		/// </summary>
		public byte[] GetRandomData(byte[] input)
		{
			lock (syncRoot)
			{
				var timeSinceLastReseed = DateTime.UtcNow - lastReseedTime;

				if (accumulator.HasEnoughEntropy && timeSinceLastReseed > MaximumTimeSpanBetweenReseeds)
				{
					reseedCounter++;
					Reseed(accumulator.GetRandomDataFromPools(reseedCounter));
					Debug.WriteLine($"Reseeding completed! Counter: {reseedCounter}");
				}

				if (reseedCounter == 0)
				{
					throw new GeneratorSeedException("Generator not seeded yet!");
				}

				return generator.GenerateData(input);
			}
		}

		/// <summary>
		/// Uses the generator to get 64 bytes of random data
		/// and turns them to a positive integer
		/// </summary>
		/// <param name="min">minimum possible value</param>
		/// <param name="max">maximum possible value</param>
		/// <returns>pseudorandomly generated positive integer</returns>
		public int GetRandomNumber(int min, int max)
		{
			byte[] result;

			byte[] input = new byte[64];
			result = GetRandomData(input);

			int num = Math.Abs(BitConverter.ToInt32(result, 0));

			return (num % (max - min)) + min;
		}

		public int[] GetRandomNumbers(int min, int max, int length)
		{
			var result = new int[length];

			for (int i = 0; i < length; i++)
			{
				result[i] = GetRandomNumber(min, max);
			}

			return result;
		}

		public string GetRandomString(int length)
		{
			throw new NotImplementedException();
		}

		public string[] GetRandomStrings(int length)
		{
			throw new NotImplementedException();
		}

		public string[] GetRandomStrings(int minStringLength, int maxStringLength, int arrayLength)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			UpdateSeed(null); 
			stream.Close();
			accumulator.Dispose();
			reseedTimer.Dispose();
		}

		#region Private implementation

		private void Reseed(byte[] seed)
		{
			generator.Reseed(seed);
			lastReseedTime = DateTimeOffset.UtcNow;
		}

		private void UpdateSeed(object state)
		{
			var latestSeed = GetRandomData(new byte[SeedFileSize]);
			Write(latestSeed);
		}

		private void Write(byte[] seed)
		{
			stream.Seek(0, SeekOrigin.Begin);
			stream.Write(seed, 0, SeedFileSize);
			stream.Flush();
		}

		#endregion
	}
}