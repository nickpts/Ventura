using System;
using System.Net;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors
{

	/// <summary>
	/// Makes a REST call to an API by csrng.net. Retrieves the maximum allowed number under the free version.
	/// </summary>
	public class NistCsrngExtractor : EntropyExtractorBase, IEntropyExtractor
	{
		public NistCsrngExtractor(IEventEmitter eventEmitter) : base(eventEmitter)
		{
		}

		protected override Func<byte[]> ExtractEntropicData()
		{
			return () =>
			{
				using (WebClient wc = new WebClient())
				{
					var jsonResponse = wc.DownloadString("https://csrng.net/csrng/csrng.php?min=10000000000000&max=1000000000000000");
					long.TryParse(jsonResponse.Substring(74, 15), out var number);
					var bytes = BitConverter.GetBytes(number);
					
					return bytes;
				}
			};
		}
	}
}
