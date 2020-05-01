using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors.Remote
{
	/// <summary>
	/// Makes a REST call to an API provided by John Walker (https://fourmilab.ch/hotbits/)
	/// The entropy comes from radioactive decay.
	/// </summary>
	internal class HotBitsExtractor : EntropyExtractorBase, IEntropyExtractor
	{
		private const string apiLink =
			"https://www.fourmilab.ch/cgi-bin/Hotbits.api?nbytes=30&fmt=json&npass=1&lpass=8&pwtype=3&apikey=HB1njZ69vSUYaLRkxNHPpjNWFnJ";

		public HotBitsExtractor(IEventEmitter eventEmitter) : base(eventEmitter)
		{
		}

		public override Func<byte[]> GetEntropicData()
		{
			return () =>
			{
				using var wc = new WebClient();

				var jsonResponse = wc.DownloadString(apiLink);

				string toBeSearched = "\"data\": ";
				jsonResponse = jsonResponse.Substring(jsonResponse.IndexOf(toBeSearched) + toBeSearched.Length);
				jsonResponse = Regex.Replace(jsonResponse, @"\n", " ");
				jsonResponse = jsonResponse.Replace('[', ' ');
				jsonResponse = jsonResponse.Replace(']', ' ');
				jsonResponse = jsonResponse.Replace('}', ' ');
				jsonResponse = jsonResponse.Replace(',', ' '); //TODO: replace these with regex

				var result = jsonResponse.Split(' ').Where(n => !string.IsNullOrEmpty(n)).Select(byte.Parse).ToArray();

				return result;
			};
		}
	}
}
