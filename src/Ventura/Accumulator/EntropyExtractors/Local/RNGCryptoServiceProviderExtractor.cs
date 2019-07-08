using System;
using System.Security.Cryptography;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors
{
	/// <summary>
	/// Uses the RNG CryptoServiceProvider to encrypt an empty array to be used as entropy
	/// </summary>
	public class RNGCryptoServiceProviderExtractor: EntropyExtractorBase, IEntropyExtractor
	{
		RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();

		public RNGCryptoServiceProviderExtractor(IEventEmitter eventEmitter) : base(eventEmitter)
		{
		}

		public override string SourceName { get; protected set; } = "RNGCryptoServiceProvider";

		protected override Func<byte[]> ExtractEntropicData()
		{
			return () =>
			{
				var bytes = new byte[30];

				_rng.GetBytes(bytes);

				return bytes;
			};
		}
	}
}
