using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

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
					
					var pressure = ExtractIntValue(4, Regex.Match(response, "pressure(.{6})").Groups[0].Value);
					var minTemp = ExtractFloatValue(6, Regex.Match(response, "temp_min(.{8})").Groups[0].Value);
					var maxTemp = ExtractFloatValue(6, Regex.Match(response, "temp_max(.{8})").Groups[0].Value);
					var sunrise = ExtractIntValue(10, Regex.Match(response, "sunrise(.{12})").Groups[0].Value);
					var humidity = ExtractIntValue(2, Regex.Match(response, "humidity(.{4})").Groups[0].Value);
					
					return BitConverter.GetBytes(pressure)
						.Concat(BitConverter.GetBytes(minTemp))
						.Concat(BitConverter.GetBytes(maxTemp))
						.Concat(BitConverter.GetBytes(sunrise))
						.Concat(BitConverter.GetBytes(humidity)).ToArray();
				}
			};
		}

		private int ExtractIntValue(int digitsToGoBack, string match)
		{
			match = match.Substring(match.Length - digitsToGoBack);

			if (int.TryParse(match, out var result))
			{
				return result;
			}

			return default;
		}

		private float ExtractFloatValue(int digitsToGoBack, string match)
		{
			match = match.Substring(match.Length - digitsToGoBack);

			if (float.TryParse(match, out var result))
			{
				return result;
			}
			return default;
		}
	}
}
