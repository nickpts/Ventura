using System;
using System.Collections.Generic;
using System.IO;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
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
                    extractors.Add(new GarbageCollectorExtractor(0));
                    extractors.Add(new AppDomainExtractor(1));
					extractors.Add(new RNGCryptoServiceProviderExtractor(2));
					extractors.Add(new ProcessEntropyExtractor(3));
                    break;
                case ReseedEntropySourceGroup.Remote:
                    extractors.Add(new RemoteQuantumRngExtractor(0));
					extractors.Add(new RemoteNistCsrngExtractor(1));
                    break;
                case ReseedEntropySourceGroup.Full:
                    extractors.Add(new GarbageCollectorExtractor(0));
                    extractors.Add(new AppDomainExtractor(1));
                    extractors.Add(new RNGCryptoServiceProviderExtractor(2));
                    extractors.Add(new ProcessEntropyExtractor(3));
					extractors.Add(new RemoteQuantumRngExtractor(4));
                    extractors.Add(new RemoteNistCsrngExtractor(5));
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