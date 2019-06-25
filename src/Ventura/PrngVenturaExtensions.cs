using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ventura
{
	internal static class PrngVenturaExtensions
	{
		public static int GetRandomNumber(this PrngVentura prng)
		{
			throw new NotImplementedException();
		}

		public static int[] GetRandomNumbers(this PrngVentura prng)
		{
			return prng.GetRandomData(new byte[100]).Select(x => (int) x).ToArray();
		}

		public static string GetRandomString(this PrngVentura prng, int length)
		{
			throw new NotImplementedException();
		}

		public static string[] GetRandomStrings(this PrngVentura prng, int length)
		{
			throw new NotImplementedException();
		}

		public static string[] GetRandomStrings(this PrngVentura prng, int minStringLength, int maxStringLength, int arrayLength)
		{
			throw new NotImplementedException();
		}
	}
}
