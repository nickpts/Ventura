using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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

            var prng = new RNGVenturaServiceProvider(new VenturaAccumulator(extractors), new VenturaGenerator(cipher), seedStream);
			prng.Initialise();

			return prng;
		}

        public static RandomNumberGenerator CreateRng(Stream seedStream, Cipher cipher, ReseedEntropySourceGroup sourceGroup)
        {
	        return (RandomNumberGenerator) Create(seedStream, cipher, sourceGroup);
        }
	}
} 