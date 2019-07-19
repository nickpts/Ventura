using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors.Remote
{
	public class WeatherEntropyExtractor: EntropyExtractorBase, IEntropyExtractor
	{
		public WeatherEntropyExtractor(IEventEmitter eventEmitter) : base(eventEmitter)
		{
		}
		
		protected override Func<byte[]> ExtractEntropicData()
		{
			return () =>
			{
				var random = new Random();
				var cityCountryPair = Constants.citiesCountries.ElementAt(random.Next(0, Constants.citiesCountries.Count));
				
				using (WebClient wc = new WebClient())
				{
					wc.Headers.Set("X-RapidAPI-Host", "community-open-weather-map.p.rapidapi.com");
					wc.Headers.Set("X-RapidAPI-Key", "5f113eddb6mshb691ece9ea3e613p10ec99jsn577de96f09b4");
					var response = wc.DownloadString($"https://community-open-weather-map.p.rapidapi.com/weather?q={cityCountryPair.Key}%2C{cityCountryPair.Value}");

					//TODO: replace with regex
					//TODO: needs more testing
					var match = Regex.Match(response, "pressure(.{6})").Groups[0].Value;
					var pressure = int.Parse(match.Substring(match.Length - 4));

					match = Regex.Match(response, "temp_min(.{8})").Groups[0].Value;
					var minTemp = int.Parse(match.Substring(match.Length - 6));

					match = Regex.Match(response, "temp_max(.{8})").Groups[0].Value;
					var maxTemp = int.Parse(match.Substring(match.Length - 6));

					match = Regex.Match(response, "sunrise(.{12})").Groups[0].Value;
					var sunrise = int.Parse(match.Substring(match.Length - 10));

					match = Regex.Match(response, "humidity(.{4})").Groups[0].Value;
					var humidity = int.Parse(match.Substring(match.Length - 2));

					return BitConverter.GetBytes(pressure)
						.Concat(BitConverter.GetBytes(minTemp))
						.Concat(BitConverter.GetBytes(maxTemp))
						.Concat(BitConverter.GetBytes(sunrise))
						.Concat(BitConverter.GetBytes(humidity)).ToArray();
				}
			};

		}
	}
}
