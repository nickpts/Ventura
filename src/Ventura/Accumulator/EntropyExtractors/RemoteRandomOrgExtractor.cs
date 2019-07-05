using System;
using System.Linq;
using System.Net;
using Ventura.Interfaces;


namespace Ventura.Accumulator.EntropyExtractors
{
	/// <summary>
	///  Makes a REST call to an API provided by Random.org
	/// </summary>
	public class RemoteRandomOrgExtractor: EntropyExtractorBase, IEntropyExtractor
	{
		public RemoteRandomOrgExtractor(int sourceNumber) : base(sourceNumber)
		{
		}
		protected override Func<byte[]> ExtractEntropicData()
		{
			return () =>
			{
				using (WebClient wc = new WebClient())
				{
					var response = wc.DownloadString("https://www.random.org/cgi-bin/randbyte?nbytes=30&format=d");

					var numberArray = response.Split(' ').Where(n => !string.IsNullOrEmpty(n)).Select(int.Parse).ToList();
					byte[] result = new byte[30];

					for (int i = 0; i < result.Length; i++)
					{
						result[i] = BitConverter.GetBytes(numberArray[i])[0];
					}

					return result;
				}
			};
		}
	}
}
