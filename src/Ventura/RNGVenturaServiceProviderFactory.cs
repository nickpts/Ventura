using System;
using System.Collections.Generic;
using System.IO;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Accumulator.EntropyExtractors.Remote;
using Ventura.Generator;
using Ventura.Interfaces;

namespace Ventura
{
    public class RNGVenturaServiceProviderFactory
	{
        public static IRNGVenturaServiceProvider Create(Stream seedStream) => Create(seedStream, Cipher.Aes, ReseedEntropySourceGroup.Local);

        public static IRNGVenturaServiceProvider Create(Stream seedStream, Cipher cipher) => Create(seedStream, cipher, ReseedEntropySourceGroup.Local);

        public static IRNGVenturaServiceProvider Create(Stream seedStream, Cipher cipher, ReseedEntropySourceGroup sourceGroup)
		{ 
	        var extractors = new List<IEntropyExtractor>();

            switch (sourceGroup)
            {
                case ReseedEntropySourceGroup.Local:
                    extractors.Add(new GarbageCollectorExtractor(new EventEmitter(0)));
                    extractors.Add(new AppDomainExtractor(new EventEmitter(1)));
					extractors.Add(new RNGCryptoServiceProviderExtractor(new EventEmitter(2)));
					extractors.Add(new ProcessEntropyExtractor(new EventEmitter(3)));
                    break;
                case ReseedEntropySourceGroup.Remote:
                    extractors.Add(new QuantumRngExtractor(new EventEmitter(0)));
					extractors.Add(new NistCsrngExtractor(new EventEmitter(1)));
					extractors.Add(new RandomOrgExtractor(new EventEmitter(2)));
                    extractors.Add(new HotBitsExtractor(new EventEmitter(3)));
					break;
                case ReseedEntropySourceGroup.Full:
                    extractors.Add(new GarbageCollectorExtractor(new EventEmitter(0)));
                    extractors.Add(new AppDomainExtractor(new EventEmitter(1)));
                    extractors.Add(new RNGCryptoServiceProviderExtractor(new EventEmitter(2)));
                    extractors.Add(new ProcessEntropyExtractor(new EventEmitter(3)));
					extractors.Add(new QuantumRngExtractor(new EventEmitter(4)));
                    extractors.Add(new NistCsrngExtractor(new EventEmitter(5)));
                    extractors.Add(new RandomOrgExtractor(new EventEmitter(6)));
                    extractors.Add(new HotBitsExtractor(new EventEmitter(7)));
					break;
                default:
	                throw new ArgumentOutOfRangeException(nameof(sourceGroup), sourceGroup, null);
            }

            var prng = new RNGVenturaServiceProvider(new VenturaAccumulator(extractors), new VenturaGenerator(cipher), seedStream);
			prng.Initialise();

			return prng;
		}
	}
} 