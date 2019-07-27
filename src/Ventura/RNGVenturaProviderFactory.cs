using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Accumulator.EntropyExtractors.Local;
using Ventura.Accumulator.EntropyExtractors.Remote;
using Ventura.Generator;
using Ventura.Interfaces;

namespace Ventura
{
    public class RNGVenturaProviderFactory
	{
		/// <summary>
		/// Initializes an instance of the PRNG with a seed consisting taken from
		/// a pseudo-randomly chosen remote entropy extractor. Cipher is AES,
		/// full entropy source groups (local remote used by default).
		/// </summary>
		public static IRNGVenturaProvider CreateSeeded() => CreateSeeded(Cipher.Aes, ReseedEntropySourceGroup.Full);

		/// <summary>
		/// Initializes an instance of the PRNG with a seed consisting taken from
		/// a pseudo-randomly chosen remote entropy extractor. Full entropy source groups
		/// (local and remote) used by default.
		/// </summary>
		public static IRNGVenturaProvider CreateSeeded(Cipher cipher) => CreateSeeded(cipher, ReseedEntropySourceGroup.Full);

		/// <summary>
		/// Initializes an instance of the PRNG with a seed consisting taken from
		/// a pseudo-randomly chosen remote entropy extractor.
		/// </summary>
		/// <param name="cipher">cipher to use</param>
		/// <param name="sourceGroup">local or remote</param>
		public static IRNGVenturaProvider CreateSeeded(Cipher cipher, ReseedEntropySourceGroup sourceGroup)
		{
			byte[] seed = ExtractEntropy();
			var seedStream = Convert(seed);

			return Create(seedStream, cipher, sourceGroup);
		}

		/// <summary>
		/// Initializes an instance of the PRNG, using AES and full entropy source groups by default.
		/// </summary>
		/// <param name="seedStream">stream containing seed information</param>
		/// <returns>initialised PRNG</returns>
		public static IRNGVenturaProvider Create(Stream seedStream) => Create(seedStream, Cipher.Aes, ReseedEntropySourceGroup.Full);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="seedStream"></param>
		/// <param name="cipher"></param>
		/// <param name="sourceGroup"></param>
		/// <returns>initialised PRNG</returns>
        public static IRNGVenturaProvider Create(Stream seedStream, Cipher cipher, ReseedEntropySourceGroup sourceGroup)
		{ 
	        var extractors = new List<IEntropyExtractor>();

            switch (sourceGroup)
            {
                case ReseedEntropySourceGroup.Local:
                    extractors.Add(new GarbageCollectorExtractor(new EventEmitter(0)));
                    extractors.Add(new AppDomainExtractor(new EventEmitter(1)));
                    extractors.Add(new ProcessEntropyExtractor(new EventEmitter(2)));
                    break;
                case ReseedEntropySourceGroup.Remote:
	                extractors.Add(new RandomOrgExtractor(new EventEmitter(0)));
                    extractors.Add(new HotBitsExtractor(new EventEmitter(1)));
					break;
                case ReseedEntropySourceGroup.Full:
                    extractors.Add(new GarbageCollectorExtractor(new EventEmitter(0)));
                    extractors.Add(new AppDomainExtractor(new EventEmitter(1)));
                    extractors.Add(new ProcessEntropyExtractor(new EventEmitter(2)));
                    extractors.Add(new RandomOrgExtractor(new EventEmitter(3)));
                    extractors.Add(new HotBitsExtractor(new EventEmitter(4)));
					break;
                default:
	                throw new ArgumentOutOfRangeException(nameof(sourceGroup), sourceGroup, null);
            }

            var prng = new RNGVenturaProvider(new VenturaAccumulator(extractors), new VenturaGenerator(cipher), seedStream);
			prng.Initialise();

			return prng;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="seedStream"></param>
		/// <param name="cipher"></param>
		/// <param name="sourceGroup"></param>
		/// <returns></returns>
        public static RandomNumberGenerator CreateRng(Stream seedStream, Cipher cipher, ReseedEntropySourceGroup sourceGroup)
        {
	        return (RandomNumberGenerator) Create(seedStream, cipher, sourceGroup);
        }

		#region Private implementation

		private static byte[] ExtractEntropy()
		{
			throw new NotImplementedException();
		}

		private static Stream Convert(byte[] seed)
		{
			var stream = new MemoryStream();
			stream.Write(seed, 0, seed.Length);

			return stream;
		}

		#endregion
	}
} 