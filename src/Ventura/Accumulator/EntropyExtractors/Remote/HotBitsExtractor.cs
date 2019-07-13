using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors.Remote
{
	public class HotBitsExtractor: EntropyExtractorBase, IEntropyExtractor
	{
		private const string apiLink =
			"https://www.fourmilab.ch/cgi-bin/Hotbits.api?nbytes=30&fmt=json&npass=1&lpass=8&pwtype=3&apikey=HB1njZ69vSUYaLRkxNHPpjNWFnJ";

		public HotBitsExtractor(IEventEmitter eventEmitter) : base(eventEmitter)
		{
		}

		protected override Func<byte[]> ExtractEntropicData()
		{
			return () =>
			{
				using (WebClient wc = new WebClient())
				{
					var jsonResponse = wc.DownloadString(apiLink);
					long.TryParse(jsonResponse.Substring(74, 15), out var number);
					var bytes = BitConverter.GetBytes(number);

					return bytes;
				}
			};
		}
	}
}
