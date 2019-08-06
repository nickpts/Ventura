using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
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
        /// Initializes an instance of the PRNG and seeds it with a pseudorandomly
        /// picked remote entropy source. Cipher is AES, all entropy source groups
        /// (local and remote used by default). A MemoryStream is used to store
        /// the seed which is discarded on exit.
        /// </summary>
        /// <returns>initialised PRNG</returns>
        public static IRNGVenturaProvider CreateSeeded() => CreateSeeded(Cipher.Aes, ReseedEntropySourceGroup.Full);

        /// <summary>
        /// Initializes an instance of the PRNG and seeds it with a pseudorandomly
        /// picked remote entropy source. All entropy source groups
        /// (local and remote used by default). A MemoryStream is used to store
        /// the seed which is discarded on exit.
        /// </summary>
        /// <returns>initialised PRNG</returns>
        public static IRNGVenturaProvider CreateSeeded(Cipher cipher) => CreateSeeded(cipher, ReseedEntropySourceGroup.Full);

        /// <summary>
        /// Initializes an instance of the PRNG and seeds it with a pseudorandomly
        /// picked remote entropy source. A MemoryStream is used to store
        /// the seed which is discarded on exit.
        /// </summary>
        /// <param name="cipher">cipher to use</param>
        /// <param name="sourceGroup">local or remote</param>
        /// <returns>initialised PRNG</returns>
        public static IRNGVenturaProvider CreateSeeded(Cipher cipher, ReseedEntropySourceGroup sourceGroup)
        {
            var extractors = new List<IEntropyExtractor>()
            {
                new WeatherEntropyExtractor(new EventEmitter(0)),
                new AtmosphericNoiseExtractor(new EventEmitter(1)),
                new HotBitsExtractor(new EventEmitter(2))
            };

            byte[] seed = new SeedProvider(extractors).GetBytes();

            return Create(Convert(seed), cipher, sourceGroup);
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
                    extractors.Add(new SystemUtcExtractor(new EventEmitter(3)));
                    break;
                case ReseedEntropySourceGroup.Remote:
	                extractors.Add(new AtmosphericNoiseExtractor(new EventEmitter(0)));
                    extractors.Add(new HotBitsExtractor(new EventEmitter(1)));
                    extractors.Add(new WeatherEntropyExtractor(new EventEmitter(2)));
                    break;
                case ReseedEntropySourceGroup.Full:
                    extractors.Add(new GarbageCollectorExtractor(new EventEmitter(0)));
                    extractors.Add(new AppDomainExtractor(new EventEmitter(1)));
                    extractors.Add(new ProcessEntropyExtractor(new EventEmitter(2)));
                    extractors.Add(new SystemUtcExtractor(new EventEmitter(3)));
                    extractors.Add(new AtmosphericNoiseExtractor(new EventEmitter(4)));
                    extractors.Add(new HotBitsExtractor(new EventEmitter(5)));
                    extractors.Add(new WeatherEntropyExtractor(new EventEmitter(6)));
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

        private static MemoryStream Convert(byte[] array)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(array, 0, array.Length);

            return stream;
        }

        #endregion
    }
} 